using UnityEngine;
using FarrokhGames.Inventory;

public class InventoryDropper : MonoBehaviour
{
    [SerializeField] private Inventory[] inventories;
    [SerializeField] private QuickSlots quickSlots;
    [SerializeField] private Transform dropPoint;

    private void Start()
    {
        foreach(Inventory inventory in inventories)
            inventory.OnItemDropped += ItemDrop;

        quickSlots.OnItemDropped += ItemDrop;
    }

    private void ItemDrop(IInventoryItem item)
    {
        Item i = (Item)item;
        GameObject go = PickupFactory.instance.GetPickup(i);
    
        go.transform.position = dropPoint.position;
        go.transform.rotation = dropPoint.rotation;

    }
}
