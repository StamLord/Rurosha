using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Pickup : PhysicalObject
{
    [SerializeField] private Item item;

    public delegate bool attemptPickupDelegate(Item item, Interactor interactor);
    public event attemptPickupDelegate OnAttemptPickup;

    public delegate void pickupDelegate(Item item);
    public event pickupDelegate OnPickup;

    public void SetItem(Item item)
    {
        this.item = item;
    }

    public override void Use(Interactor interactor)
    {
        base.Use(interactor);

        if(item)
        {   
            if(OnAttemptPickup != null)
            {
                bool success = OnAttemptPickup(item, interactor);
                if(success == false) return;
            }

            if(interactor.WeaponManager.AddItem(item))
            {
                if(OnPickup != null)
                    OnPickup(item);
                
                // If we have child pickups,
                // we unparent them so they are not destroyed
                Pickup[] childPickups = GetComponentsInChildren<Pickup>();
                foreach(Pickup p in childPickups)
                {
                    if(p == this) continue;
                    p.transform.SetParent(transform.parent);
                    p.SetRigidActive(true);
                }

                Destroy(transform.gameObject);
            }
        }
    }

    // Used by Grappling Hook to pick up items
    public void Use(WeaponManager manager)
    {
        if(item)
            if(manager.AddItem(item))
                Destroy(transform.gameObject);
    }
}
