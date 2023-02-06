using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProj : MonoBehaviour, IHitboxResponder
{
    [Header("Bomb Settings")]
    [SerializeField] private bool autoStartTimer = true;
    [SerializeField] private float countdown = 3;
    [SerializeField] private float radius = 5;
    [SerializeField] private DamageType damageType = DamageType.Pierce;
    [SerializeField] private int softDamage = 10;
    [SerializeField] private int hardDamage = 20;
    [SerializeField] private float explosionForce = 20;
    [SerializeField] private float explosionUpForce = 10;
    
    [Header("References")]
    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Hitbox hitbox;

    private bool started;
    private float startTime;
    private List<GameObject> objectsCollided = new List<GameObject>();

    void Start()
    {
        hitbox.SetResponder(this);
        if(autoStartTimer) 
            StartTimer();
    }

    public void StartTimer()
    {
        started = true;
        startTime = Time.time;
    }

    void Update()
    {
        if(started == false)
            return;
        
        if(Time.time >= startTime + countdown)
            Explode();
    }
    
    private void Explode()
    {
        StopTimer();
        if(visual) visual.SetActive(false);
        if(explosion)
        {
            explosion.transform.SetParent(null);
            explosion.transform.rotation = Quaternion.identity;
            explosion.SetActive(true);
        }
            
        hitbox.StartColliding(true);
    }

    private void StopTimer()
    {
        started = false;
    }

    public void CollisionWith(Collider col, Hitbox hitbox)
    {
        // Get Hurtbox
        Hurtbox hurtbox = col.GetComponent<Hurtbox>();

        // Check if we have line of sight to collider
        // RaycastHit hit;
        // if(Physics.Raycast(transform.position, col.transform.position - transform.position, out hit, radius))
        // {
        //     // If this is something we can hurt, we want only colliders not under the same root to block explosion
        //     // Otheriwise, any other object (aka transform) will block it
        //     if(hurtbox) 
        //         if(hit.transform.root != col.transform.root)
        //             return;
        //     else
        //         if(hit.transform != col.transform)
        //             return;
        // }

        // Register hit
        if(hurtbox)
        {
            // Avoid triggering multiple hurtboxes with the same parent GameObject
            if(objectsCollided.Contains(hurtbox.transform.parent.gameObject) == false)
            {
                hurtbox.Hit(null, softDamage, hardDamage, Vector3.up, damageType);
                objectsCollided.Add(hurtbox.transform.parent.gameObject);
            } 
        }

        // Get rigidbody and apply explosion force
        Rigidbody rb = col.GetComponent<Rigidbody>();
        if(rb)
            rb.AddExplosionForce(explosionForce, transform.position, radius, explosionUpForce, ForceMode.Impulse);
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
