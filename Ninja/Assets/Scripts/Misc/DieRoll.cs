using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice 
{
    public int RollDice(int sides)
    {
        return Random.Range(1, sides);
    }

    public int[] RollDie(int amount, int sides)
    {
        int[] die = new int[amount];
        
        for(int i=0; i < amount; i++)
            die[i] = Random.Range(1, sides);
            
        return die;
    }
}
