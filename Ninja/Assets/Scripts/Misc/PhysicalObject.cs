using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeightClass {LIGHT, MEDIUM, HEAVY}

[RequireComponent(typeof(Outline))]
public class PhysicalObject : Usable
{
    [SerializeField] private WeightClass weightClass;

    public WeightClass GetWeight()
    {
        return weightClass;
    }
}
