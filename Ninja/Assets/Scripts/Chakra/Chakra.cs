using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chakra
{
    [SerializeField] private ChakraType type;
    [SerializeField] private float amount;
    [SerializeField] private int full;
    
    public ChakraType Type {get {return type;}}
    public float Amount {get {return amount;}}
    public int Full {get {return full;}}

    public bool Add(float value)
    {
        amount += value;
        bool overflow = amount + value > 1f;

        while(amount >= 1f)
        {
            amount -= 1f;
            full++;
        }

        return overflow;
    }

    public bool Remove(float value)
    {
        amount -= value;
        bool overflow = amount - value < 0f;

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
