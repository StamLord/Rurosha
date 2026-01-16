using UnityEngine;
using FarrokhGames.Inventory;

public class QuickSlots : QuickSlotsBase
{
    [Header("Inventory Renderers")]
    [SerializeField] private InventoryRenderer[] renderers;
    [SerializeField] private UIWindow window;

    [SerializeField] private ItemDefinition[] autoLoadItems;

    private InventoryManager[] managers;

    #region Events

    public override event itemsUpdated OnItemsUpdated;
    public override event itemAddDelegate OnItemAdd;
    public override event itemRemoveDelegate OnItemRemove;
    public override event itemDropdelegate OnItemDropped;

    #endregion
    
    // Each slot is a signle item
    private InventoryRenderMode renderMode = InventoryRenderMode.Single;

    public override int Length()
    {
        return renderers.Length;
    }

    private void Start() 
    {
        window.Open();

        managers = new InventoryManager[renderers.Length];

        for (var i = 0; i < managers.Length; i++)
        {
            // Create manager
            GridInventoryProvider provider = new GridInventoryProvider(renderMode, 1);
            managers[i] = new InventoryManager(provider, 1, 1);
            managers[i].onItemAdded += HandleItemsUpdate;
            managers[i].onItemRemoved += HandleItemsUpdate;
            managers[i].onItemDropped += HandleItemDrop;

            // Set renderer
            renderers[i].SetInventory(managers[i], renderMode);
        }

         for (int i = 0; i < autoLoadItems.Length; i++)
        {
            if (i > managers.Length)
                break;
            Debug.Log("Adding " + autoLoadItems[i]);
            bool success = managers[i].TryAddAt(autoLoadItems[i], Vector2Int.zero);
            Debug.Log(success);
        }

        window.Close();
    }
    
    // Index getter

    public override IInventoryItem GetItem(int index)
    {
        return managers[index].GetAtPoint(Vector2Int.zero);
    }

    public override IInventoryItem this[int index]
    { 
        get { return managers[index].GetAtPoint(Vector2Int.zero); } 
    }

    public override bool RemoveItem(int index)
    {
        if(OnItemRemove != null) OnItemRemove(index);
        if(OnItemsUpdated != null) OnItemsUpdated();

        return managers[index].TryRemove(managers[index].GetAtPoint(Vector2Int.zero));
    }

    public override bool AddItemAtFirstEmpty(IInventoryItem item)
    {
        for(int i = 0; i < Length(); i++)
        {
            // 1 item per slot so if not full, it's empty
            if(managers[i].isFull == false)
            {
                bool success = managers[i].TryAdd(item);
                if(success)
                {
                    if(OnItemAdd != null) OnItemAdd(i);
                    if(OnItemsUpdated != null) OnItemsUpdated();
                    return true;
                }
            }
        }
        
        return false;        
    }

    public override bool AddItem(int index, IInventoryItem item)
    {
        if(OnItemAdd != null) OnItemAdd(index);
        if(OnItemsUpdated != null) OnItemsUpdated();

        return managers[index].TryAdd(item);
    }

    private void HandleItemsUpdate(IInventoryItem item)
    {
        if(OnItemsUpdated != null) OnItemsUpdated();
    }

    private void HandleItemDrop(IInventoryItem item)
    {
        if(OnItemDropped != null) OnItemDropped(item); 
    }
}
