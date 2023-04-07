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
    
    private void Start()
    {
        // Open inventory window
        inventoryWindow.Open();

        // Set inventory UI
        var provider = new GridInventoryProvider(renderMode);
        manager = new InventoryManager(provider, inventoryWidth, inventoryHeight);
        inventoryRenderer?.SetInventory(manager, InventoryRenderMode.Grid);

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
        return manager.TryAdd(item);
    }
}
