using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : WeaponObject
{
    [SerializeField] private Consumable consumable;
    private float durabilityLossPerSecond;
    private float fractionPerSecond;

    void Update()
    {
        if(inputState.MouseButton1.Pressed)
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
            manager.AddItemAtSelection(consumable.leftoverItem);
        else
            manager.DepleteItem();
    }

    private void ActivateConsumableEffects(float fraction)
    {
        charStats.AddHealth(consumable.healthRestore * fraction);
        charStats.AddPotentialHealth(consumable.potentialHealthRestore * fraction);
        charStats.AddStamina(consumable.staminaRestore * fraction);
        charStats.AddPotentialStamina(consumable.potentialStaminaRestore * fraction);
    }

    public override void SetItem(Item item)
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
}

