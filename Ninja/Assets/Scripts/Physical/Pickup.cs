using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Pickup : PhysicalObject
{
    [SerializeField] private Item _item;

    public override void Use(Interactor interactor)
    {
        base.Use(interactor);

        if(_item)
        {
            if(interactor.weaponManager.AddItem(_item))
               Destroy(transform.gameObject);
        }
    }
}
