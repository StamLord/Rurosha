using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour, IHitboxResponder
{
    [Header("Reference")]
    [SerializeField] private StealthAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Hitbox[] hitbox;
    
    [Header("Physics")]
    [SerializeField] private float kickForce = 5f;
    [SerializeField] private float enemyKickForce = 15f;
    [SerializeField] private float kickUpwardsModifier = 1f;
    
    [Header("Damage")]
    [SerializeField] private int softDamage = 5;
    [SerializeField] private int hardDamage = 10;
    [SerializeField] private DamageType damageType = DamageType.Blunt;

    private void Start() 
    {
        foreach (Hitbox h in hitbox)
        {
            h.SetIgnoreTransform(transform.root);
            h.SetResponder(this);
        }
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        // Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox)
        {
            Vector3 force = transform.forward * kickForce + Vector3.up * kickUpwardsModifier;
            hurtbox.Hit(agent, softDamage, hardDamage, Vector3.up, force, damageType);
        }

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

    public void PerfectGuardedBy(Collider collider, Hitbox hitbox)
    {
        
    }

    public void UpdateColliderState(bool state)
    {

    }

    public void ActivateKick(Vector3 movement)
    {
        if(movement.z < 0)              // Back
            animator.Play("snap_kick", 2);
        else if(movement.x < 0)         // Left
            animator.Play("right_kick", 2);
        else if(movement.x > 0)         // Right
            animator.Play("left_kick", 2);
        else                            // Forward
            animator.Play("push_kick", 2);
    }

    public void AirKick()
    {
        animator.Play("flying_kick", 2);
    }
}
