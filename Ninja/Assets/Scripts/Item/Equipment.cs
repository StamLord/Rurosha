using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Items/Equipment", order = 3)]
public class Equipment : Item
{
    public EquipmentType equipmentType;
    public EquipmentLayer equipmentLayer;

    public int visualIndex;

    public int bluntDefense;
    public int slashDefense;
    public int pierceDefense;

    public int palette;
    public int pattern;

    public override void Randomize()
    {
        base.Randomize();
        palette = RandomEquipmentManager.instance.GetRandomPalette();
        pattern = RandomEquipmentManager.instance.GetRandomPattern();
    }
}

