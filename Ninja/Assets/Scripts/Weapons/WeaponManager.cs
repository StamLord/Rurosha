using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats _characterStats;
    public CharacterStats Stats {get {return _characterStats;}}

    [SerializeField] private StealthAgent _stealthAgent;
    public StealthAgent Agent {get {return _stealthAgent;}}

    [SerializeField] private Camera _camera;
    public Camera Camera {get {return _camera;}}

    [SerializeField] private InputState _inputState;
    public InputState InputState {get {return _inputState;}}

    [SerializeField] private Transform dropOrigin;
    
    [Header("Items")]
    [SerializeField] private Item[] items = new Item[10];
    [SerializeField] private int selected = 0;
    int oldSelected;

    [Header("Ammo")]
    [SerializeField] Dictionary<string, int> ammo = new Dictionary<string, int>();
    
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
    [SerializeField] private GameObject _makibishi;
    [SerializeField] private GameObject _bow;
    [SerializeField] private GameObject _grappling_hook;
    [SerializeField] private GameObject _item;
    [SerializeField] private GameObject _itemBowl;
    [SerializeField] private GameObject _equipment;

    [SerializeField] private GameObject _lastActive;

    public delegate void ChangeSelectionDelegate(int index);
    public event ChangeSelectionDelegate ChangeSelectionEvent;

    public delegate void ChangeCursorDelegate(CursorType cursor);
    public event ChangeCursorDelegate ChangeCursorEvent;

    public delegate void ChangeItemDeleget(int index, Item item, int stack = 0);
    public event ChangeItemDeleget ChangeItemEvent;

    public static Dictionary<string, Item> itemDatabase = new Dictionary<string, Item>();

    void Start()
    {
        // Prepare weapon objects
        if(_melee)_melee.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_knife)_knife.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_sword)_sword.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_staff)_staff.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_kanabo)_kanabo.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_shuriken)_shuriken.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_big_shuriken)_big_shuriken.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_bomb)_bomb.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_makibishi)_makibishi.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_bow)_bow.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_grappling_hook)_grappling_hook.GetComponent<WeaponObject>().SetWeaponManager(this);

        if(_equipment)_equipment.GetComponent<WeaponObject>().SetWeaponManager(this);
        
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
                if(itemDatabase.ContainsKey(parameters[0]) == false)
                    return "Unknown weapon: " + parameters[0];

                int num;
                if(int.TryParse(parameters[1], out num) == false)
                    return "Parameter is not a number:" + parameters[1];
                
                for (int i = 0; i < Int32.Parse(parameters[1]); i++)
                    AddItem(itemDatabase[parameters[0].ToLower()]);
                
                return "Added weapon: " + parameters[0] + " x" + parameters[1];
        }));
    }

    private void Update()
    {
        oldSelected = selected;

        #region Mouse Wheel

        if(_inputState.ScrollInput > 0f)
        {
            if(selected >= items.Length -1 /*&& weapons[0]*/) 
                selected = 0;
            else /*if(weapons[selected + 1])*/
                selected++;
        }

        if(_inputState.ScrollInput < 0f)
        {
            if(selected <= 0 /*&& weapons[weapons.Length - 1]*/) 
                selected = items.Length -1;
            else if(selected > 0 /*&& weapons[selected - 1]*/)
                selected--;
        }

        #endregion
        
        #region Num Keys
        
        if(_inputState.Num1.State == VButtonState.PRESS_START)
            selected = 0;
        if(_inputState.Num2.State == VButtonState.PRESS_START)
            selected = 1;
        if(_inputState.Num3.State == VButtonState.PRESS_START)
            selected = 2;
        if(_inputState.Num4.State == VButtonState.PRESS_START)
            selected = 3;
        if(_inputState.Num5.State == VButtonState.PRESS_START)
            selected = 4;
        if(_inputState.Num6.State == VButtonState.PRESS_START)
            selected = 5;
        if(_inputState.Num7.State == VButtonState.PRESS_START)
            selected = 6;
        if(_inputState.Num8.State == VButtonState.PRESS_START)
            selected = 7;
        if(_inputState.Num9.State == VButtonState.PRESS_START)
            selected = 8;
        if(_inputState.Num0.State == VButtonState.PRESS_START)
            selected = 9;
        
        #endregion

        #region Drop

        if(_inputState.Drop.State == VButtonState.PRESS_START)
            DropItem();
        
        #endregion

        if(selected != oldSelected)
        {
            SelectItem();
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }   
    }

    // Perform the selection, disable/enable appropriate objects
    private void SelectItem()
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
                case WeaponType.MELEE:
                    ActivateObject(_melee);
                    break;
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
                case WeaponType.MAKIBISHI:
                    ActivateObject(_makibishi);
                    break;
                case WeaponType.BOW:
                    ActivateObject(_bow);
                    break;
                case WeaponType.GRAPPLING_HOOK:
                    ActivateObject(_grappling_hook);
                    break;
            }
        }
        // Equipment
        else if (items[selected] is Equipment)
        {
            ActivateObject(_equipment);
        }
        else // Item
        {
            if(items[selected].itemName == ("Rice") || items[selected].itemName == ("Ramen"))
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

    // Activate gameobject (a held weapon with his own script)
    private void ActivateObject(GameObject gameObject)
    {
        gameObject?.SetActive(true);
        _lastActive = gameObject;
        if(items[selected])
        {
            WeaponObject wo = gameObject.GetComponent<WeaponObject>();
            if(wo)
                wo.SetItem(items[selected]); // Change all references to WeaponObject so we don't need to search for component
        }
    }

    // Select best weapon based on damage
    public void SelectBestWeapon()
    {
        int damage = 0;
        int index = 0;

        for (var i = 0; i < items.Length; i++)
        {
            if(items[i] == null) continue;
            if(items[i].GetType() == typeof(Weapon))
            {   
                Weapon w = (Weapon)items[i];
                if(w.damage > damage)
                {
                    damage = w.damage;
                    index = i;
                }
            }
        }
        oldSelected = selected;
        selected = index;
        if(selected != oldSelected)
        {
            SelectItem();
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }   
    }

    public void AddAmmo(Ammo item)
    {
        if(ammo.ContainsKey(item.itemName))
            ammo[item.itemName] += item.ammo;
        else
            ammo[item.itemName] = item.ammo;
    }

    public bool RemoveAmmo(string name, int amount = 1)
    {
        if(ammo.ContainsKey(name) == false)
            return false;

        if(ammo[name] < amount)
            return false;
            
        ammo[name] -= amount;
        return true;
    }

    public bool AddItem(Item item)
    {
        // Add and exit if it's ammo
        if(item is Ammo)
        {
            AddAmmo((Ammo)item);
            return true;
        }

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
                SelectItem();
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
            SelectItem();
            return true;
        }

        return false;
    }

    public void AddItemAtSelection(Item item)
    {   
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

    public void DropItem()
    {
        // Create pickup object
        Instantiate(items[selected].pickup, dropOrigin.position, Quaternion.identity);

        // Remove selected item
        if(items[selected].stackable)
            DepleteItem();
        else
            RemoveItem();
    }

    public int GetAmmo()
    {
        return items[selected].ammo;
    }

    public Item GetSelectedItem()
    {
        return items[selected];
    }

    public GameObject GetActiveGameObject()
    {
        return _lastActive;
    }

    public void SetCursor(CursorType cursorType)
    {
        if(ChangeCursorEvent != null)
            ChangeCursorEvent(cursorType);
    }
}
