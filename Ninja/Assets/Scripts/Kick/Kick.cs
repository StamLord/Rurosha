using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour, IHitboxResponder
{
    [Header("Reference")]
    [SerializeField] private StealthAgent agent;
    
    [Header("Physics")]
    [SerializeField] private float kickForce = 5f;
    [SerializeField] private float kickUpwardsModifier = 1f;
    [SerializeField] private float kickRadius = 1f;
    
    [Header("Damage")]
    [SerializeField] private int softDamage = 5;
    [SerializeField] private int hardDamage = 10;
    [SerializeField] private DamageType damageType = DamageType.Blunt;
    [SerializeField] private Hitbox hitbox;

    private void Start() 
    {
        hitbox.SetIgnoreTransform(transform.root);
        hitbox.SetResponder(this);
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        // Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox)
            hurtbox.Hit(agent, softDamage, hardDamage, damageType);

        // Rigidbody
        Rigidbody rb = collider.GetComponent<Rigidbody>();

        if(rb == null)
            rb = collider.transform.parent.GetComponent<Rigidbody>();
        
        if(rb)
        {
            Vector3 closestPoint = collider.ClosestPoint(hitbox.Position);
            Vector3 dir = transform.forward;
            rb.AddForceAtPosition(dir * kickForce + Vector3.up * kickUpwardsModifier, closestPoint, ForceMode.Impulse);
        }
    }

    public void GuardedBy(Collider collider, Hitbox hitbox)
    {
        
    }

    public void UpdateColliderState(bool state)
    {

    }

    public void ActivateKick()
    {
        hitbox.StartColliding(true);
    }
}
