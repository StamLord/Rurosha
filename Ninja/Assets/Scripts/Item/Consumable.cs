using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Items/Consumable", order = 4)]
public class Consumable : Item
{
    public int healthRestore;
    public int potentialHealthRestore;

    public int staminaRestore;
    public int potentialStaminaRestore;

    public float consumeDuration = 5;
    public bool incremental = true;

    public Item leftoverItem;
}
