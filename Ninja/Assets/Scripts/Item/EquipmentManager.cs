using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Equipped")]
    [SerializeField] private Equipment head;
    [SerializeField] private Equipment torso;
    [SerializeField] private Equipment legs;
    [SerializeField] private Equipment arms;
    [SerializeField] private Equipment feet;

    [SerializeField] private Equipment head2;
    [SerializeField] private Equipment torso2;
    [SerializeField] private Equipment legs2;
    [SerializeField] private Equipment arms2;
    [SerializeField] private Equipment feet2;

    [Header("Visual")]
    [SerializeField] private GameObject[] vHead;
    [SerializeField] private GameObject[] vTorso;
    [SerializeField] private GameObject[] vLegs;
    [SerializeField] private GameObject[] vArms;
    [SerializeField] private GameObject[] vFeet;

    private int lastVHead;
    private int lastVTorso;
    private int lastVLegs;
    private int lastVArms;
    private int lastVFeet;

    private int lastVHead2;
    private int lastVTorso2;
    private int lastVLegs2;
    private int lastVArms2;
    private int lastVFeet2;

    public struct Defense
    {
        public int bluntDefense;
        public int slashDefense;
        public int pierceDefense;
    }

    public string GetEquipmentName(EquipmentType type, EquipmentLayer layer)
    {
        switch(type)
        {
            case EquipmentType.Head:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        if(head) return head.itemName;
                        break;
                    case EquipmentLayer.OVER:
                        if(head2) return head2.itemName;
                        break;
                }
                break;
            case EquipmentType.Torso:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        if(torso) return torso.itemName;
                        break;
                    case EquipmentLayer.OVER:
                        if(torso2) return torso2.itemName;
                        break;
                }
                break;
            case EquipmentType.Arms:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        if(arms) return arms.itemName;
                        break;
                    case EquipmentLayer.OVER:
                        if(arms2) return arms2.itemName;
                        break;
                }
                break;
            case EquipmentType.Legs:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        if(legs) return legs.itemName;
                        break;
                    case EquipmentLayer.OVER:
                        if(legs2) return legs2.itemName;
                        break;
                }
                break;
            case EquipmentType.Feet:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        if(feet) return feet.itemName;
                        break;
                    case EquipmentLayer.OVER:
                        if(feet2) return feet2.itemName;
                        break;
                }
                break;
        }
        
        return "";
    }

    public Equipment Equip(Equipment equipment)
    {
        Equipment oldEquipment = null;

        switch(equipment.equipmentType)
        {
            case EquipmentType.Head:
                switch(equipment.equipmentLayer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = head;
                        head = equipment;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = head2;
                        head2 = equipment;
                        break;
                }
                break;
            case EquipmentType.Torso:
                switch(equipment.equipmentLayer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = torso;
                        torso = equipment;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = torso2;
                        torso2 = equipment;
                        break;
                }
                break;
            case EquipmentType.Legs:
                switch(equipment.equipmentLayer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = legs;
                        legs = equipment;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = legs2;
                        legs2 = equipment;
                        break;
                }
                break;
            case EquipmentType.Arms:
                switch(equipment.equipmentLayer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = arms;
                        arms = equipment;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = arms2;
                        arms2 = equipment;
                        break;
                }
                break;
            case EquipmentType.Feet:
                switch(equipment.equipmentLayer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = feet;
                        feet = equipment;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = feet2;
                        feet2 = equipment;
                        break;
                }
                break;
        }

        UpdateVisual(equipment.equipmentType, equipment.equipmentLayer);
        return oldEquipment;
    }

    public Equipment UnEquip(EquipmentType slot, EquipmentLayer layer)
    {
        Equipment oldEquipment = null;

        switch(slot)
        {
            case EquipmentType.Head:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = head;
                        head = null;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = head2;
                        head2 = null;
                        break;
                }
                break;
            case EquipmentType.Torso:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = torso;
                        torso = null;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = torso2;
                        torso2 = null;
                        break;
                }
                break;
            case EquipmentType.Legs:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = legs;
                        legs = null;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = legs2;
                        legs2 = null;
                        break;
                }
                break;
            case EquipmentType.Arms:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = arms;
                        arms = null;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = arms2;
                        arms2 = null;
                        break;
                }
                break;
            case EquipmentType.Feet:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        oldEquipment = feet;
                        feet = null;
                        break;
                    case EquipmentLayer.OVER:
                        oldEquipment = feet2;
                        feet2 = null;
                        break;
                }
                break;
        }

        UpdateVisual(slot, layer);
        return oldEquipment;
    }

    private void UpdateVisual(EquipmentType slot, EquipmentLayer layer)
    {
        switch(slot)
        {
            case EquipmentType.Head:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        vHead[lastVHead].SetActive(false);
                        if(head)
                        {
                            vHead[head.visualIndex].SetActive(true);
                            UpdateMaterial(vHead[head.visualIndex], head.palette, head.pattern);
                            lastVHead = head.visualIndex;
                        }
                        break;
                    case EquipmentLayer.OVER:
                        vHead[lastVHead2].SetActive(false);
                        if(head2)
                        {
                            vHead[head2.visualIndex].SetActive(true);
                            UpdateMaterial(vHead[head2.visualIndex], head2.palette, head2.pattern);
                            lastVHead2 = head2.visualIndex;
                        }
                        break;
                }
                break;
            case EquipmentType.Torso:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        vTorso[lastVTorso].SetActive(false);
                        if(torso)
                        {
                            vTorso[torso.visualIndex].SetActive(true);
                            UpdateMaterial(vTorso[torso.visualIndex], torso.palette, torso.pattern);
                            lastVTorso = torso.visualIndex;
                        }
                        break;
                    case EquipmentLayer.OVER:
                        vTorso[lastVTorso2].SetActive(false);
                        if(torso2)
                        {
                            vTorso[torso2.visualIndex].SetActive(true);
                            UpdateMaterial(vTorso[torso2.visualIndex], torso2.palette, torso2.pattern);
                            lastVTorso2 = torso2.visualIndex;
                        }
                        break;
                }
                break;
            case EquipmentType.Legs:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        vLegs[lastVLegs].SetActive(false);
                        if(legs)
                        {
                            vLegs[legs.visualIndex].SetActive(true);
                            UpdateMaterial(vLegs[legs.visualIndex], legs.palette, legs.pattern);
                            lastVLegs = legs.visualIndex;
                        }
                        break;
                    case EquipmentLayer.OVER:
                        vLegs[lastVLegs2].SetActive(false);
                        if(legs2)
                        {
                            vLegs[legs2.visualIndex].SetActive(true);
                            UpdateMaterial(vLegs[legs2.visualIndex], legs2.palette, legs2.pattern);
                            lastVLegs2 = legs2.visualIndex;
                        }
                        break;
                }
                break;
            case EquipmentType.Arms:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        vArms[lastVArms].SetActive(false);
                        if(arms)
                        {
                            vArms[arms.visualIndex].SetActive(true);
                            UpdateMaterial(vArms[arms.visualIndex], arms.palette, arms.pattern);
                            lastVArms = arms.visualIndex;
                        }
                        break;
                    case EquipmentLayer.OVER:
                        vArms[lastVArms2].SetActive(false);
                        if(arms2)
                        {
                            vArms[arms2.visualIndex].SetActive(true);
                            UpdateMaterial(vArms[arms2.visualIndex], arms2.palette, arms2.pattern);
                            lastVArms2 = arms2.visualIndex;
                        }
                        break;
                }
                break;
            case EquipmentType.Feet:
                switch(layer)
                {
                    case EquipmentLayer.UNDER:
                        vFeet[lastVFeet].SetActive(false);
                        if(feet)
                        {
                            vFeet[feet.visualIndex].SetActive(true);
                            UpdateMaterial(vFeet[feet.visualIndex], feet.palette, feet.pattern);
                            lastVFeet = feet.visualIndex;
                        }
                        break;
                    case EquipmentLayer.OVER:
                        vFeet[lastVFeet2].SetActive(false);
                        if(feet2)
                        {
                            vFeet[feet2.visualIndex].SetActive(true);
                            UpdateMaterial(vFeet[feet2.visualIndex], feet2.palette, feet2.pattern);
                            lastVFeet2 = feet2.visualIndex;
                        }
                        break;
                }
                break;
        }
    }

    private void UpdateMaterial(GameObject gameObject, int palette, int pattern)
    {
        SkinnedMeshRenderer sm = gameObject.GetComponent<SkinnedMeshRenderer>();
        RandomEquipmentManager.Palette p = RandomEquipmentManager.instance.GetPalette(palette);

        sm.material.SetColor("_Main_Color", p.primary);
        sm.material.SetColor("_Secondary_Color", p.secondary);
        sm.material.SetTexture("_Secondary_Pattern", RandomEquipmentManager.instance.GetPattern(pattern));
    }

    public Defense GetDefense()
    {
        Defense defense = new Defense();

        // Layer 1
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

        // Layer 2
        if(head2)
        {
            defense.bluntDefense += head2.bluntDefense;
            defense.slashDefense += head2.slashDefense;
            defense.pierceDefense += head2.pierceDefense;
        }

        if(torso2)
        {
            defense.bluntDefense += torso2.bluntDefense;
            defense.slashDefense += torso2.slashDefense;
            defense.pierceDefense += torso2.pierceDefense;
        }

        if(legs2)
        {
            defense.bluntDefense += legs2.bluntDefense;
            defense.slashDefense += legs2.slashDefense;
            defense.pierceDefense += legs2.pierceDefense;
        }

        if(arms2)
        {
            defense.bluntDefense += arms2.bluntDefense;
            defense.slashDefense += arms2.slashDefense;
            defense.pierceDefense += arms2.pierceDefense;
        }

        if(feet2)
        {
            defense.bluntDefense += feet2.bluntDefense;
            defense.slashDefense += feet2.slashDefense;
            defense.pierceDefense += feet2.pierceDefense;
        }

        return defense;
    }
}
