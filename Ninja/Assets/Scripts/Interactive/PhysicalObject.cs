using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class PhysicalObject : Usable
{
    [SerializeField] private WeightClass weightClass;
    [SerializeField] private new Rigidbody rigidbody;

    public WeightClass GetWeight()
    {
        return weightClass;
    }

    public void SetRigidActive(bool active)
    {
        if(rigidbody)
            rigidbody.isKinematic = !active;
    }
}
