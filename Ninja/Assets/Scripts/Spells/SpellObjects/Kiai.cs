using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiai : SpellObject, IHitboxResponder
{
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private float force;
    [SerializeField] private ParticleSystem vfx;

    private void Start()
    {
        hitbox.SetResponder(this);
    }

    public override void Activate(SpellManager manager)
    {
        vfx.Play();
        hitbox.StartColliding(true);
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Rigidbody r = collider.GetComponent<Rigidbody>();
        if(r)
            r.AddForce(transform.forward * force, ForceMode.Impulse);
    }

    public void UpdateColliderState(bool state)
    {

    }
}
