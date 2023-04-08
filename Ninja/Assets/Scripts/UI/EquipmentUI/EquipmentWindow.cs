using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentWindow : UIWindow
{
    [SerializeField] private GameObject container;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private Interactor interactor;
    [SerializeField] private Transform dropOrigin;

    [SerializeField] private TextMeshProUGUI head;
    [SerializeField] private TextMeshProUGUI torso;
    [SerializeField] private TextMeshProUGUI legs;
    [SerializeField] private TextMeshProUGUI arms;
    [SerializeField] private TextMeshProUGUI feet;

    [SerializeField] private TextMeshProUGUI head2;
    [SerializeField] private TextMeshProUGUI torso2;
    [SerializeField] private TextMeshProUGUI legs2;
    [SerializeField] private TextMeshProUGUI arms2;
    [SerializeField] private TextMeshProUGUI feet2;

    [SerializeField] private TextMeshProUGUI blunt;
    [SerializeField] private TextMeshProUGUI slash;
    [SerializeField] private TextMeshProUGUI pierce;

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(IsOpen)
                Close();
            else
            {
                UpdateVisual();
                Open();
            }

            container.SetActive(IsOpen);
        }
    }

    private void UpdateVisual()
    {
        head.text = equipmentManager.GetEquipmentName(EquipmentType.Head, EquipmentLayer.UNDER);
        torso.text = equipmentManager.GetEquipmentName(EquipmentType.Torso, EquipmentLayer.UNDER);
        arms.text = equipmentManager.GetEquipmentName(EquipmentType.Arms, EquipmentLayer.UNDER);
        legs.text = equipmentManager.GetEquipmentName(EquipmentType.Legs, EquipmentLayer.UNDER);
        feet.text = equipmentManager.GetEquipmentName(EquipmentType.Feet, EquipmentLayer.UNDER);

        head2.text = equipmentManager.GetEquipmentName(EquipmentType.Head, EquipmentLayer.OVER);
        torso2.text = equipmentManager.GetEquipmentName(EquipmentType.Torso, EquipmentLayer.OVER);
        arms2.text = equipmentManager.GetEquipmentName(EquipmentType.Arms, EquipmentLayer.OVER);
        legs2.text = equipmentManager.GetEquipmentName(EquipmentType.Legs, EquipmentLayer.OVER);
        feet2.text = equipmentManager.GetEquipmentName(EquipmentType.Feet, EquipmentLayer.OVER);

        UpdateStats();
    }

    private void UnEquip(EquipmentType type, EquipmentLayer layer)
    {
        Equipment old = equipmentManager.UnEquip(type, layer);
        
        if(old) 
        {
            // Try to add item and create drop if not able 
            if(interactor.AddItem(old) == false)
            {
                GameObject go = PickupFactory.instance.GetPickup(old);
                go.transform.position = dropOrigin.position;
                go.transform.rotation = Quaternion.identity;
            }
        }
        
        UpdateVisual();
    }

    public void UnEquipHelmet()
    {
        UnEquip(EquipmentType.Head, EquipmentLayer.UNDER);
    }

    public void UnEquipTorso()
    {
        UnEquip(EquipmentType.Torso, EquipmentLayer.UNDER);
    }

    public void UnEquipLegs()
    {
        UnEquip(EquipmentType.Legs, EquipmentLayer.UNDER);
    }

    public void UnEquipArms()
    {
        UnEquip(EquipmentType.Arms, EquipmentLayer.UNDER);
    }

    public void UnEquipFeet()
    {
        UnEquip(EquipmentType.Feet, EquipmentLayer.UNDER);
    }

    public void UnEquipHelmet2()
    {
        UnEquip(EquipmentType.Head, EquipmentLayer.OVER);
    }

    public void UnEquipTorso2()
    {
        UnEquip(EquipmentType.Torso, EquipmentLayer.OVER);
    }

    public void UnEquipLegs2()
    {
        UnEquip(EquipmentType.Legs, EquipmentLayer.OVER);
    }

    public void UnEquipArms2()
    {
        UnEquip(EquipmentType.Arms, EquipmentLayer.OVER);
    }

    public void UnEquipFeet2()
    {
        UnEquip(EquipmentType.Feet, EquipmentLayer.OVER);
    }

    private void UpdateStats()
    {
        EquipmentManager.Defense def = equipmentManager.GetDefense();
        Debug.Log(def);
        blunt.text = "" + def.bluntDefense;
        slash.text = "" + def.slashDefense;
        pierce.text = "" + def.pierceDefense;
    }
}
