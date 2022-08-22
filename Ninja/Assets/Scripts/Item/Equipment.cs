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

    public int color1;
    public int color2;
    public int pattern;
    public int patternColor;

    public override void Randomize()
    {
        base.Randomize();
        color1 = RandomEquipmentManager.instance.GetRandomColor();
        color2 = RandomEquipmentManager.instance.GetRandomColor();
        pattern = RandomEquipmentManager.instance.GetRandomPattern();
        patternColor = RandomEquipmentManager.instance.GetRandomColor();
    }
}

