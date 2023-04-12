using System;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Inventory;

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

    [SerializeField] private Rigidbody _rigidbody;
    public Rigidbody Rigidbody {get {return _rigidbody;}}

    [SerializeField] private Transform dropOrigin;
    
    [Header("Items")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private QuickSlots slots;
    [SerializeField] private int selected = 0;
    int oldSelected;

    [Header("Ammo")]
    [SerializeField] List<Ammo> ammo = new List<Ammo>();
    
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
    [SerializeField] private GameObject _scroll;

    [SerializeField] private GameObject _lastActive;

    public delegate void ChangeSelectionDelegate(int index);
    public event ChangeSelectionDelegate ChangeSelectionEvent;

    public delegate void ChangeCursorDelegate(CursorType cursor);
    public event ChangeCursorDelegate ChangeCursorEvent;

    public delegate void ChangeItemDeleget(int index, Item item, int stack = 0);
    public event ChangeItemDeleget ChangeItemEvent;

    public delegate void ChangeWeaponDelegate(WeaponType weaponType);
    public event ChangeWeaponDelegate ChangeWeaponEvent;

    public static Dictionary<string, Item> itemDatabase = new Dictionary<string, Item>();

    private void Start()
    {
        if(inventory) inventory.OnInventoryUpdate += InventoryUpdate;
        if(slots) slots.OnItemsUpdated += UpdateSelection;

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

        if(_item)_item.GetComponent<WeaponObject>().SetWeaponManager(this);
        if(_itemBowl)_itemBowl.GetComponent<WeaponObject>().SetWeaponManager(this);

        if(_equipment)_equipment.GetComponent<WeaponObject>().SetWeaponManager(this);
        
        if(_scroll) _scroll.GetComponent<WeaponObject>().SetWeaponManager(this);
        
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

        // DebugCommandDatabase.AddCommand(new DebugCommand(
        //     "addweapon",
        //     "Adds a weapon to player",
        //     "addweapon <weapon> <amount>", 
        //     (string[] parameters) => {
        //         if(itemDatabase.ContainsKey(parameters[0]) == false)
        //             return "Unknown weapon: " + parameters[0];

        //         int num;
        //         if(int.TryParse(parameters[1], out num) == false)
        //             return "Parameter is not a number:" + parameters[1];
                
        //         for (int i = 0; i < Int32.Parse(parameters[1]); i++)
        //             AddItem(itemDatabase[parameters[0].ToLower()]);
                
        //         return "Added weapon: " + parameters[0] + " x" + parameters[1];
        // }));
    }

    private void Update()
    {
        oldSelected = selected;

        #region Mouse Wheel

        if(_inputState.ScrollInput > 0f)
        {
            if(selected >= slots.Length -1 /*&& weapons[0]*/) 
                selected = 0;
            else /*if(weapons[selected + 1])*/
                selected++;
            
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }

        if(_inputState.ScrollInput < 0f)
        {
            if(selected <= 0 /*&& weapons[weapons.Length - 1]*/) 
                selected = slots.Length -1;
            else if(selected > 0 /*&& weapons[selected - 1]*/)
                selected--;
            
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }

        #endregion
        
        #region Num Keys
        
        if(_inputState.Num1.State == VButtonState.PRESS_START)
        {
            selected = 0;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num2.State == VButtonState.PRESS_START)
        {
            selected = 1;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num3.State == VButtonState.PRESS_START)
        {
            selected = 2;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num4.State == VButtonState.PRESS_START)
        {
            selected = 3;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num5.State == VButtonState.PRESS_START)
        {
            selected = 4;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num6.State == VButtonState.PRESS_START)
        {
            selected = 5;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num7.State == VButtonState.PRESS_START)
        {
            selected = 6;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num8.State == VButtonState.PRESS_START)
        {
            selected = 7;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num9.State == VButtonState.PRESS_START)
        {
            selected = 8;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        if(_inputState.Num0.State == VButtonState.PRESS_START)
        {
            selected = 9;
            if(ChangeSelectionEvent != null) ChangeSelectionEvent(selected);
        }
        
        #endregion

        #region Drop

        if(_inputState.Drop.State == VButtonState.PRESS_START)
            DropItem();
        
        #endregion

        if(selected != oldSelected)
        {
            SelectItem();
            
        }   
    }

    private void UpdateSelection()
    {
        SelectItem();
    }

    // Perform the selection, disable/enable appropriate objects
    private void SelectItem()
    {
        if(_lastActive)
            _lastActive.SetActive(false);
        
        if (slots[selected] == null)
        {
            ActivateObject(_melee);
        }
        // Weapon
        else if(slots[selected] is Weapon)
        {   
            Weapon w = (Weapon)slots[selected];
            switch(w.WeaponType)
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
            if(ChangeWeaponEvent != null)
                ChangeWeaponEvent(w.WeaponType);
        }
        // Equipment
        else if (slots[selected] is Equipment)
        {
            ActivateObject(_equipment);
        }
        //Scroll
        else if (slots[selected] is Scroll)
        {
            ActivateObject(_scroll);
        }
        // Item
        else if (slots[selected] is Item)
        {
            Item item = (Item)slots[selected];

            if(item.itemName == ("Rice") || item.itemName == ("Ramen"))
            {
                ActivateObject(_itemBowl);
                _itemBowl?.GetComponent<HeldItem>().SetItem(item);
            }
            else
            {
                ActivateObject(_item);
                _item?.GetComponent<HeldItem>().SetItem(item);
            }
        }
        
    }

    // Activate gameobject (a held weapon with his own script)
    private void ActivateObject(GameObject gameObject)
    {
        gameObject?.SetActive(true);
        _lastActive = gameObject;
        if(slots[selected] != null)
        {
            WeaponObject wo = gameObject.GetComponent<WeaponObject>();
            if(wo)
                wo.SetItem((Item)slots[selected]); // Change all references to WeaponObject so we don't need to search for component
        }
    }

    // Select best weapon based on damage
    public void SelectBestWeapon()
    {
        int damage = 0;
        int index = 0;

        for (var i = 0; i < slots.Length; i++)
        {
            if(slots[i] == null) continue;
            if(slots[i].GetType() == typeof(Weapon))
            {   
                Weapon w = (Weapon)slots[i];
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
        inventory.TryAdd(item);
    }

    public bool RemoveAmmo(string name, int amount = 1)
    {
        // Try to find ammo
        foreach(Ammo a in ammo)
        {
            if(a.itemName == name)
            {
                // Not enough
                if(a.amount < amount)
                    return false;
                
                // Deplete
                a.amount -= amount;

                // Remove item if amount reaches 0
                if(a.amount < 1)
                    inventory.TryRemove(a);
                
                return true;
            }
        }

        // No ammo found
        return false;
    }

    public void AddItemAtSelection(Item item)
    {   
        if(slots[selected] != null)
            RemoveItem();
        
        slots.AddItem(selected, item);
        if(ChangeItemEvent != null) ChangeItemEvent(selected, item);

        SelectItem();
    }

    public void RemoveItem()
    {
        RemoveItem(selected);
    }

    public void RemoveItem(int index)
    {
        slots.RemoveItem(index);
        if(ChangeItemEvent != null) ChangeItemEvent(index, null);
    }

    public void DepleteItem()
    {
        DepleteItem(1);
    }

    public void DepleteItem(int amount)
    {
        Item item = (Item)slots[selected];
        item.amount -= amount;
        item.durability = 100f;

        if(item.amount < 1)
            RemoveItem(selected);
        else
        {
            if(ChangeItemEvent != null) ChangeItemEvent(selected, item, item.amount);
        }
    }

    public void DropItem()
    {
        if(slots[selected] == null) return;

        Item item = (Item)slots[selected];
        
        GameObject go = PickupFactory.instance.GetPickup(item);
        go.transform.position = dropOrigin.position;
        go.transform.rotation = Quaternion.identity;

        // Remove selected item
        if(item.stackable)
            DepleteItem();
        else
            RemoveItem();
    }
    
    /// <summary>
    /// Drops the item at the same position and rotation as the visual gameobject for that item
    /// </summary>
    public void DropItemNPC()
    {
        if(slots[selected] == null) return;

        Item item = (Item)slots[selected];

        GameObject go = PickupFactory.instance.GetPickup(item);
        go.transform.position = _lastActive.transform.position;
        go.transform.rotation = _lastActive.transform.rotation;

        // Remove selected item
        if(item.stackable)
            DepleteItem();
        else
            RemoveItem();
    }

    public int GetAmount()
    {
        return ((Item)slots[selected]).amount;
    }

    public List<Ammo> GetAmmoInInventory()
    {
        IInventoryItem[] items = inventory.AllItems;
        List<Ammo> ammo = new List<Ammo>();
        
        // Find items with type of ammo
        for (var i = 0; i < items.Length; i++)
        {
            if(items[i] is Ammo)
                ammo.Add((Ammo)items[i]);
        }

        return ammo;
    }

    private void InventoryUpdate()
    {
        ammo = GetAmmoInInventory();
    }

    public Item GetSelectedItem()
    {
        return (Item)slots[selected];
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
