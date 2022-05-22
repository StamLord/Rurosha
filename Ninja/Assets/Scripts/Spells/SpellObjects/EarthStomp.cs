using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthStomp : SpellObject, IHitboxResponder
{
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private float upForce= 1f;
    [SerializeField] private float upVariance = .2f;
    [SerializeField] private float outForce = 5f;
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
        float upVar = 1 + Random.Range(-upVariance, upVariance);
        if(r)
            r.AddForce(Vector3.up * upForce * upVar + (collider.transform.position - transform.position) * outForce, ForceMode.Impulse);
    }

    public void UpdateColliderState(bool state)
    {

    }
}
