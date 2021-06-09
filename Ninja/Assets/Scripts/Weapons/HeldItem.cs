using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Animator animator;

    [SerializeField] private Item item;
    [SerializeField] private Consumable consumable;
    private float durabilityLossPerSecond;
    private float fractionPerSecond;

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            animator?.SetBool("Using", true);
            if(consumable)
            {
                if(consumable.durability > 0)
                {
                    consumable.durability -= durabilityLossPerSecond * Time.deltaTime;
                    if(consumable.incremental)
                        ActivateConsumableEffects(fractionPerSecond * Time.deltaTime);
                }
                else
                    ConsumeItem();
            }
        }
        else
        {
            animator?.SetBool("Using", false);
        }
    }

    void ConsumeItem()
    {
        if(consumable.incremental == false)
            ActivateConsumableEffects(1f);

        if(consumable.leftoverItem) 
            weaponManager.AddItemAtSelection(consumable.leftoverItem);
        else
            weaponManager.DepleteItem();
    }

    void ActivateConsumableEffects(float fraction)
    {
        characterStats.AddHealth(consumable.healthRestore * fraction);
        characterStats.AddPotentialHealth(consumable.potentialHealthRestore * fraction);
        characterStats.AddStamina(consumable.staminaRestore * fraction);
        characterStats.AddPotentialStamina(consumable.potentialStaminaRestore * fraction);
    }

    public void SetItem(Item item)
    {
        this.item = item;
        if(item is Consumable)
        {
          consumable = (Consumable)item;
          durabilityLossPerSecond = consumable.durability / consumable.consumeDuration;
          fractionPerSecond = 1 / consumable.consumeDuration;
        }
        else
            consumable = null;

        UpdateVisual();
    }

    void UpdateVisual()
    {
        if(meshFilter) meshFilter.mesh = item.model;
        if(meshRenderer) meshRenderer.material = item.material;
    }
}

