using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class PhysicalObject : Usable
{
    [SerializeField] private WeightClass weightClass;

    public WeightClass GetWeight()
    {
        return weightClass;
    }
}
