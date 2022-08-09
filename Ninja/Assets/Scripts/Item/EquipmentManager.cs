using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] Equipment head;
    [SerializeField] Equipment torso;
    [SerializeField] Equipment legs;
    [SerializeField] Equipment arms;
    [SerializeField] Equipment feet;

    public struct Defense
    {
        public int bluntDefense;
        public int slashDefense;
        public int pierceDefense;
    }

    public Equipment Equip(Equipment equipment)
    {
        Equipment oldEquipment = null;

        switch(equipment.equipmentType)
        {
            case EquipmentType.Head:
                
                oldEquipment = head;
                head = equipment;

                break;
            case EquipmentType.Torso:

                oldEquipment = torso;
                torso = equipment;

                break;
            case EquipmentType.Legs:

                oldEquipment = legs;
                legs = equipment;

                break;
            case EquipmentType.Arms:

                oldEquipment = arms;
                arms = equipment;

                break;
            case EquipmentType.Feet:

                oldEquipment = feet;
                feet = equipment;

                break;
        }

        return oldEquipment;
    }

    public Equipment UnEquip(EquipmentType slot)
    {
        Equipment oldEquipment = null;

        switch(slot)
        {
            case EquipmentType.Head:
                oldEquipment = head;
                head = null;
                break;
            case EquipmentType.Torso:
                oldEquipment = torso;
                torso = null;
                break;
            case EquipmentType.Legs:
                oldEquipment = legs;
                legs = null;
                break;
            case EquipmentType.Arms:
                oldEquipment = arms;
                arms = null;
                break;
            case EquipmentType.Feet:
                oldEquipment = feet;
                feet = null;
                break;
        }

        return oldEquipment;
    }

    public Defense GetDefense()
    {
        Defense defense = new Defense();

        if(head)
        {
            defense.bluntDefense += head.bluntDefense;
            defense.slashDefense += head.slashDefense;
            defense.pierceDefense += head.pierceDefense;
        }

        if(torso)
        {
            defense.bluntDefense += torso.bluntDefense;
            defense.slashDefense += torso.slashDefense;
            defense.pierceDefense += torso.pierceDefense;
        }

        if(legs)
        {
            defense.bluntDefense += legs.bluntDefense;
            defense.slashDefense += legs.slashDefense;
            defense.pierceDefense += legs.pierceDefense;
        }

        if(arms)
        {
            defense.bluntDefense += arms.bluntDefense;
            defense.slashDefense += arms.slashDefense;
            defense.pierceDefense += arms.pierceDefense;
        }

        if(feet)
        {
            defense.bluntDefense += feet.bluntDefense;
            defense.slashDefense += feet.slashDefense;
            defense.pierceDefense += feet.pierceDefense;
        }

        return defense;
    }
}
