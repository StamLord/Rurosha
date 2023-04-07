using UnityEngine;
using TMPro;
using FarrokhGames.Inventory;

public class GridInventorySelection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private InventoryController controller;

    void Start()
    {
        if(controller)
            controller.onItemHovered += HandleItemHover;
    }

    private void HandleItemHover(IInventoryItem item)
    {
        if (item != null)
            text.text = (item as ItemDefinition).Name;
        else
            text.text = string.Empty;
    }
}
