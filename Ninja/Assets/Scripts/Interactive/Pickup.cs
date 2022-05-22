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
            if(interactor.WeaponManager.AddItem(_item))
                Destroy(transform.gameObject);
        }
    }

    public void Use(WeaponManager manager)
    {
        if(_item)
            if(manager.AddItem(_item))
                Destroy(transform.gameObject);
    }
}
