using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chakra
{
    [SerializeField] private ChakraType type;
    [SerializeField] private float amount;
    [SerializeField] private int full;
    [SerializeField] private int maxFull = 1;
    
    public ChakraType Type {get {return type;}}
    public float Amount {get {return amount;}}
    public float TotalAmount { get {return amount + full;}}
    public int Full {get {return full;}}

    public bool Add(float value)
    {
        amount += value;
        bool overflow = amount + value > 1f;

        // Calculate full rounds
        while(amount >= 1f && full < maxFull -1)
        {
            amount -= 1f;
            full++;
        }

        // Clamp in case above loop did not trigger due to being on last full round
        if(amount > 1f) amount = 1f;

        return overflow;
    }

    public bool Remove(float value)
    {
        amount -= value;
        bool overflow = amount - value <= 0f;

        while(amount <= 0)
        {
            if(full > 0)
            {
                full--;
                amount += 1f;
            }
            else
            {
                amount = 0f;
                break;
            }
        }

        return overflow;
    }
}
