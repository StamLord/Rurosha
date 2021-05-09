using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {Blunt, Slash, Pierce}
public enum EquipmentType {Head, Torso, Legs, Arms, Feet}

[CreateAssetMenu(fileName = "Equipment", menuName = "Items/Equipment", order = 3)]
public class Equipment : Item
{
    public EquipmentType equipmentType;

    public int bluntDefense;
    public int slashDefense;
    public int pierceDefense;
}

