using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : WeaponObject
{
    [SerializeField] private EquipmentManager equipmentManager;

    void Update()
    {
        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
        {
            Equipment old = equipmentManager.Equip((Equipment)item);
            if(old)
                manager.AddItemAtSelection(old);
            else
                manager.RemoveItem();
        }
    }
}
