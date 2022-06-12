using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakraRecharge : Usable
{
    [SerializeField] private ChakraType type;
    [SerializeField] private float amount;
    [SerializeField] private float maxAmount;
    [SerializeField] private float fillRate;
    [SerializeField] private ParticleSystem vfx;

    public override void Use(Interactor interactor)
    {
        base.Use(interactor);
        if(amount < maxAmount) return;
        interactor.ChargeChakra(type, amount);
        amount = 0;
        vfx.Play();
    }

    private void Update() 
    {
        amount += fillRate * Time.deltaTime;
        amount = Mathf.Clamp(amount, 0, maxAmount);
    }
}
