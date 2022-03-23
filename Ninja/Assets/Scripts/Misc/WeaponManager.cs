using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private CharacterStats _characterStats;
    public CharacterStats Stats {get {return _characterStats;}}
    
    [SerializeField] private Transform weaponsHolder;
    [SerializeField] private Item[] items = new Item[10];
    [SerializeField] private int selected = 0;
    
    [Header("Default Weapon")]
    [SerializeField] private Weapon defaultWeapon;

    [Header("Templates")]
    [SerializeField] private GameObject _melee;
    [SerializeField] private GameObject _knife;
    [SerializeField] private GameObject _sword;
    [SerializeField] private GameObject _staff;
    [SerializeField] private GameObject _kanabo;
    [SerializeField] private GameObject _shuriken;
    [SerializeField] private GameObject _big_shuriken;
    [SerializeField] private GameObject _bomb;
    [SerializeField] private GameObject _bow;
    [SerializeField] private GameObject _item;
    [SerializeField] private GameObject _itemBowl;
    [SerializeField] private GameObject _equipment;

    [SerializeField] private WeaponType _lastType;
    [SerializeField] private GameObject _lastActive;

    public delegate void ChangeSelectionDeleget(int index);
    public event ChangeSelectionDeleget ChangeSelectionEvent;

    public delegate void ChangeItemDeleget(int index, Item item, int stack = 0);
    public event ChangeItemDeleget ChangeItemEvent;

    public static Dictionary<string, Item> itemDatabase = new Dictionary<string, Item>();

    void Start()
    {
        SelectItem();

        // Move later to its own class
        
        Item[] items = new Item[] 
        {
            //(Item)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Items/Weapons/Shuriken.asset", typeof(Item))
        };

        foreach (Item i in items)
        {
            itemDatabase[i.itemName.ToLower()] = i;
        }

        DebugCommandDatabase.AddCommand(new DebugCommand(
            "addweapon",
            "Adds a weapon to player",
            "addweapon <weapon> <amount>", 
            (string[] parameters) => {
                for (int i = 0; i < Int32.Parse(parameters[1]); i++)
                    AddItem(itemDatabase[parameters[0].ToLower()]);
        }));
    }

    void Update()
    {
        int oldSelected = selected;

        #region Mouse Wheel

        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(selected >= items.Length -1 /*&& weapons[0]*/) 
                selected = 0;
            else /*if(weapons[selected + 1])*/
                selected++;
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if(selected <= 0 /*&& weapons[weapons.Length - 1]*/) 
                selected = items.Length -1;
            else if(selected > 0 /*&& weapons[selected - 1]*/)
                selected--;
        }

        #endregion
        
        #region Num Keys

        if(Input.GetKeyDown(KeyCode.Alpha1))
            selected = 0;
        if(Input.GetKeyDown(KeyCode.Alpha2))
            selected = 1;
        if(Input.GetKeyDown(KeyCode.Alpha3))
            selected = 2;
        if(Input.GetKeyDown(KeyCode.Alpha4))
            selected = 3;
        if(Input.GetKeyDown(KeyCode.Alpha5))
            selected = 4;
        if(Input.GetKeyDown(KeyCode.Alpha6))
            selected = 5;
        if(Input.GetKeyDown(KeyCode.Alpha7))
            selected = 6;
        if(Input.GetKeyDown(KeyCode.Alpha8))
            selected = 7;
        if(Input.GetKeyDown(KeyCode.Alpha9))
            selected = 8;
        if(Input.GetKeyDown(KeyCode.Alpha0))
            selected = 9;
        
        #endregion

        if(selected != oldSelected)
        {
            SelectItem();
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }   
    }

    void SelectItem()
    {
        if(_lastActive)
            _lastActive.SetActive(false);
        
        if(items[selected] == null)
            items[selected] = Instantiate(defaultWeapon);
        
        // Weapon
        if(items[selected] is Weapon)
        {
            switch(((Weapon)items[selected]).WeaponType)
            {
                case WeaponType.KNIFE:
                    ActivateObject(_knife);
                    break;
                case WeaponType.SWORD:
                    ActivateObject(_sword);
                    break;
                case WeaponType.STAFF:
                    ActivateObject(_staff);
                    break;
                case WeaponType.KANABO:
                    ActivateObject(_kanabo);
                    break;
                case WeaponType.SHURIKEN:
                    ActivateObject(_shuriken);
                    break;
                case WeaponType.BIG_SHURIKEN:
                    ActivateObject(_big_shuriken);
                    break;
                case WeaponType.BOMB:
                    ActivateObject(_bomb);
                    break;
                case WeaponType.BOW:
                    break;
            }
        }
        // Equipment
        else if (items[selected] is Equipment)
        {
            ActivateObject(_equipment);
            _equipment?.GetComponent<EquipmentItem>().SetEquipment((Equipment)items[selected]);
        }
        else // Item
        {
            if(items[selected].itemName == ("Rice Bowl") || items[selected].itemName == ("Ramen Bowl"))
            {
                ActivateObject(_itemBowl);
                _itemBowl?.GetComponent<HeldItem>().SetItem(items[selected]);
            }
            else
            {
                ActivateObject(_item);
                _item?.GetComponent<HeldItem>().SetItem(items[selected]);
            }
            
        }
    }

    void ActivateObject(GameObject gameObject)
    {
        gameObject?.SetActive(true);
        _lastActive = gameObject;
    }

    public bool AddItem(Item item)
    {
        int firstEmpty = -1;

        // Same Loop is used to check for stackable 
        // items and the first empty slot
        for(int i = 0; i < items.Length; i++)
        {
            if(items[i] == null && i != 0 || items[i].itemName == defaultWeapon.itemName && i != 0)
            {
                if(firstEmpty == -1)
                    firstEmpty = i; // We keep record of the first empty slot so we don't loop again later
            }
            // If not empty, check if same and stackable
            else if(item.itemName == items[i].itemName &&  
                item.stackable && 
                items[i].stackable)
            {
                items[i].ammo += item.ammo;
                if(ChangeItemEvent != null) ChangeItemEvent(i, item, items[i].ammo);
                return true;
            }
        }
        
        if(firstEmpty != -1)
        {
            // if(item is Weapon)
            //     items[firstEmpty] = Instantiate((Weapon)item);
            // else if(item is Equipment)
            //     items[firstEmpty] = Instantiate((Equipment)item);
            // else
                items[firstEmpty] = Instantiate(item);

            if(ChangeItemEvent != null) ChangeItemEvent(firstEmpty, item);
            return true;
        }

        return false;
    }

    public void AddItemAtSelection(Item item)
    {   Debug.Log("Adding item " + item);
        items[selected] = item;
        if(ChangeItemEvent != null) ChangeItemEvent(selected, item);

        SelectItem();
    }

    public void RemoveItem()
    {
        items[selected] = null;
        if(ChangeItemEvent != null) ChangeItemEvent(selected, null);

        SelectItem();
    }

    public void RemoveItem(int index)
    {
        items[index] = null;
        if(ChangeItemEvent != null) ChangeItemEvent(index, null);

        SelectItem();
    }

    public void DepleteItem()
    {
        DepleteItem(1);
    }

    public void DepleteItem(int amount)
    {
        Item item = items[selected];
        item.ammo -= amount;
        item.durability = 100f;

        if(item.ammo < 1)
            RemoveItem(selected);
        else
            if(ChangeItemEvent != null) ChangeItemEvent(selected, item, item.ammo);

    }

    public int GetAmmo()
    {
        return items[selected].ammo;
    }
}
