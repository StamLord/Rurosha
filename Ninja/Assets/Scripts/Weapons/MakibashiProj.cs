using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakibashiProj : MonoBehaviour, IHitboxResponder
{
    [Header("Rigidbody")]
    [SerializeField] private Rigidbody rigidbody;

    [Header("Hitbox")]
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private float maxVelocityActive = .1f;
    [SerializeField] private float minVelocityHurt = .1f;

    [SerializeField] private int softDamage;
    [SerializeField] private int hardDamage;
    [SerializeField] private DamageType damageType;

    private List<GameObject> objectsCollided = new List<GameObject>();

    private void Start()
    {
        if(hitbox) hitbox.SetResponder(this);
    }
    
    private void Update()
    {
        if(rigidbody.velocity.magnitude <= maxVelocityActive)
            hitbox.StartColliding();
        else
            hitbox.StopColliding();
    }

    public void CollisionWith(Collider col, Hitbox hitbox)
    {
        // Get Hurtbox
        Hurtbox hurtbox = col.GetComponent<Hurtbox>();
        Rigidbody rb = col.transform.root.GetComponentInChildren<Rigidbody>();

        if(hurtbox)
        {
            if(rb)
            {
                // Register hit if rigidbody moves fast enough
                if(rb.velocity.magnitude > minVelocityHurt)
                    hurtbox.Hit(null, softDamage, hardDamage, Vector3.up, Vector3.zero, damageType);
            }
            // Fallback if no rigidbody found
            else
                hurtbox.Hit(null, softDamage, hardDamage, Vector3.up, Vector3.zero, damageType);
        }
    }

    public void UpdateColliderState(bool state)
    {
        if(state == false)
            objectsCollided.Clear();
    }

    public void GuardedBy(Collider collider, Hitbox hitbox)
    {
        // Play guarded animation
        // Depelte stamina
        // Stun if run out of stamina
    }

    public void PerfectGuardedBy(Collider collider, Hitbox hitbox)
    {

    }
}
