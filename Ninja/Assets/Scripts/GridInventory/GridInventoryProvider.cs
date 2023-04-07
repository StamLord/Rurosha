using System.Collections.Generic;
using FarrokhGames.Inventory;

public class GridInventoryProvider : IInventoryProvider
{
    private List<IInventoryItem> _items = new List<IInventoryItem>();
    private int _maximumAlowedItemCount;

    /// <summary>
    /// CTOR
    /// </summary>
    public GridInventoryProvider(InventoryRenderMode renderMode, int maximumAlowedItemCount = -1)
    {
        inventoryRenderMode = renderMode;
        _maximumAlowedItemCount = maximumAlowedItemCount;
    }

    public int inventoryItemCount => _items.Count;

    public InventoryRenderMode inventoryRenderMode { get; private set; }

    public bool isInventoryFull
    {
        get
        {
            if (_maximumAlowedItemCount < 0)return false;
            return inventoryItemCount >= _maximumAlowedItemCount;
        }
    }

    public bool AddInventoryItem(IInventoryItem item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
            return true;
        }
        return false;
    }

    public bool DropInventoryItem(IInventoryItem item)
    {
        return RemoveInventoryItem(item);
    }

    public IInventoryItem GetInventoryItem(int index)
    {
        return _items[index];
    }

    public bool CanAddInventoryItem(IInventoryItem item)
    {
        return true;
    }

    public bool CanRemoveInventoryItem(IInventoryItem item)
    {
        return true;
    }

    public bool CanDropInventoryItem(IInventoryItem item)
    {
        return true;
    }

    public bool RemoveInventoryItem(IInventoryItem item)
    {
        return _items.Remove(item);
    }
}
