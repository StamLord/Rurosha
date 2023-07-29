using UnityEngine;
using FarrokhGames.Inventory;

public class NpcSlots : QuickSlotsBase
{
    [SerializeField] ItemDefinition[] _items = new ItemDefinition[10];
    [SerializeField] IInventoryItem[] items = new IInventoryItem[10];

    #region Events

    public override event itemsUpdated OnItemsUpdated;
    public override event itemAddDelegate OnItemAdd;
    public override event itemRemoveDelegate OnItemRemove;
    public override event itemDropdelegate OnItemDropped;

    #endregion

    // Turned serialized ItemDefinitions[] (Configured in editor) to the needed IInventoryItem[]
    private void Awake() 
    {
        for (var i = 0; i < _items.Length; i++)
        {
            items[i] = _items[i];
        }
    }

    public override int Length()
    {
        return items.Length;
    }
    
    public override IInventoryItem this[int index]
    {
        get 
        {
            return items[index];
        }
    }

    public override IInventoryItem GetItem(int index)
    {
        return items[index];
    }

    public override bool RemoveItem(int index)
    {
        if(items[index] != null)
        {
            items[index] = null;
            return true;
        }

        return false;
    }

    public override bool AddItem(int index, IInventoryItem item)
    {
        if(items[index] == null)
        {
            items[index] = item;
            return true;
        }

        return false;
    }

    public override bool AddItemAtFirstEmpty(IInventoryItem item)
    {
        for (var i = 0; i < items.Length; i++)
        {
            if(items[i] == null)
            {
                items[i] = item;
                return true;
            }
        }

        return false;
    }
}
