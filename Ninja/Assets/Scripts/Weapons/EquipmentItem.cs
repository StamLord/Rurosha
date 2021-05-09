using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : MonoBehaviour
{
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private MeshFilter meshFilter;

    [SerializeField] private Equipment equipment;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Equipment old = equipmentManager.Equip(equipment);
            if(old)
                weaponManager.AddItemAtSelection(old);
            else
                weaponManager.RemoveItem();
        }
    }

    public void SetEquipment(Equipment equipment)
    {
        this.equipment = equipment;
        Debug.Log(equipment);
        UpdateVisual();
    }

    void UpdateVisual()
    {
        meshFilter.mesh = equipment.model;
    }
}
