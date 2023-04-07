using UnityEngine;
using FarrokhGames.Inventory;
using System;

public class QuickSlots : MonoBehaviour
{
    [Header("Inventory Renderers")]
    [SerializeField] private InventoryRenderer[] renderers;
    [SerializeField] private UIWindow window;

    private InventoryManager[] managers;

    public int Length => renderers.Length;
    
    // Each slot is a signle item
    private InventoryRenderMode renderMode = InventoryRenderMode.Single;

    public delegate void itemsUpdated();
    public event itemsUpdated OnItemsUpdated;

    public delegate void itemAddDelegate(int index);
    public event itemAddDelegate OnItemAdd;

    public delegate void itemRemoveDelegate(int index);
    public event itemRemoveDelegate OnItemRemove;

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

            // Set renderer
            renderers[i].SetInventory(managers[i], renderMode);
        }

        window.Close();
    }
    
    // Index getter
    public IInventoryItem this[int index]
    {
        get 
        {
            return managers[index].GetAtPoint(Vector2Int.zero);
        }
    }

    public bool RemoveItem(int index)
    {
        if(OnItemRemove != null) OnItemRemove(index);
        if(OnItemsUpdated != null) OnItemsUpdated();

        return managers[index].TryRemove(managers[index].GetAtPoint(Vector2Int.zero));
    }

    public bool AddItemAtFirstEmpty(IInventoryItem item)
    {
        for(int i = 0; i < Length; i++)
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

    public bool AddItem(int index, IInventoryItem item)
    {
        if(OnItemAdd != null) OnItemAdd(index);
        if(OnItemsUpdated != null) OnItemsUpdated();

        return managers[index].TryAdd(item);
    }

    private void HandleItemsUpdate(IInventoryItem item)
    {
        if(OnItemsUpdated != null) OnItemsUpdated();
    }
}
