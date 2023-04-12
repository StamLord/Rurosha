using UnityEngine;
using FarrokhGames.Inventory;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Renderer")]
    [SerializeField] private InventoryRenderer inventoryRenderer;
    [SerializeField] private UIWindow inventoryWindow;

    [Header("Inventory Settings")]
    [SerializeField] private InventoryRenderMode renderMode;
    [SerializeField] private int inventoryWidth = 7;
    [SerializeField] private int inventoryHeight = 12;

    [Header("Debug")]
    [SerializeField] private ItemDefinition[] definitions;
    [SerializeField] private bool fillRandom;

    private InventoryManager manager;
    
    public delegate void itemDropDelegate(IInventoryItem item);
    public event itemDropDelegate OnItemDropped;

    public delegate void inventoryUpdateDelegate();
    public event inventoryUpdateDelegate OnInventoryUpdate;
    
    private void Start()
    {
        // Open inventory window
        inventoryWindow.Open();

        // Set inventory UI
        var provider = new GridInventoryProvider(renderMode);
        manager = new InventoryManager(provider, inventoryWidth, inventoryHeight);
        inventoryRenderer?.SetInventory(manager, InventoryRenderMode.Grid);

        manager.onItemDropped += HandleItemDrop;
        manager.onItemAdded += HandleItemAdded;
        manager.onItemRemoved += HandleItemRemoved;

        // Close inventory window
        inventoryWindow.Close();
        
        // Randomize for tests
        if(fillRandom)
        {
            var tries = (inventoryWidth * inventoryHeight) / 3;
            for (var i = 0; i < tries; i++)
                manager.TryAdd(definitions[UnityEngine.Random.Range(0, definitions.Length)].CreateInstance());
        }
    }

    public bool TryAdd(IInventoryItem item)
    {
        // If item is stackable, find an existing item in inventory and add to it's amount
        if(item.stackable)
        {
            IInventoryItem[] items = manager.allItems;

            for (var i = 0; i < items.Length; i++)
            {   
                if(items[i].name == item.name)
                {
                    items[i].amount += item.amount;
                    
                    if(OnInventoryUpdate != null) OnInventoryUpdate(); 
                    return true;
                }
            }
        }

        return manager.TryAdd(item);
    }

    public bool TryRemove(IInventoryItem item)
    {
        return manager.TryRemove(item);
    }

    private void HandleItemDrop(IInventoryItem item)
    {
        if(OnItemDropped != null) OnItemDropped(item); 
    }

    private void HandleItemAdded(IInventoryItem item)
    {
        if(OnInventoryUpdate != null) OnInventoryUpdate(); 
    }

    private void HandleItemRemoved(IInventoryItem item)
    {
        if(OnInventoryUpdate != null) OnInventoryUpdate(); 
    }

    public IInventoryItem[] AllItems => manager.allItems;
}
