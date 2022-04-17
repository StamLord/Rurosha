using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IHitboxResponder
{   
    [Header("Damage")]
    [SerializeField] private int softDamage = 5;
    [SerializeField] private int hardDamage = 5;

    [Header("Visual")]
    [SerializeField] private Vector3 visualRotationPerSecond;
    [SerializeField] private Vector3 parentRotationPerSecond;
    [SerializeField] private GameObject visual;
    [SerializeField] private Vector3 minRotation;
    [SerializeField] private Vector3 maxRotation;
    [SerializeField] private float penetration = -.1f;

    [Header("Trajectory")]
    [SerializeField] private float speed;
    [SerializeField] private bool stopped;

    [SerializeField] private bool lastPositionCheck;
    [SerializeField] private Vector3 lastPosition;
    [SerializeField] private RaycastHit hitDetected;

    public LayerMask hitMask;

    [Header("Hitbox")]
    [SerializeField] private Hitbox[] hitbox;

    [Header ("Pickupable")]
    [SerializeField] private bool pickupable;
    
    [Tooltip("Will enable this pickup when stopped")]
    [SerializeField] private GameObject pickup;

    [Header ("Collider")]
    [Tooltip("Will enable this collider when stopped")]
    [SerializeField] private new Collider collider;
    [SerializeField] private List<GameObject> objectsCollided = new List<GameObject>();

    void Start()
    {
        lastPosition = transform.position;

        for(int i=0; i < hitbox.Length; i++)
        {
            hitbox[i].SetResponder(this);
            hitbox[i].StartColliding();
        }

    }

    void Update()
    {
        if(stopped) return;
        
        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;
        
        // Rotate visual object
        if(visual) 
        {
            //visual.transform.Rotate(visualRotationPerSecond.x * Time.deltaTime, 0, 0, Space.Self);
            visual.transform.RotateAround(visual.transform.right, visualRotationPerSecond.x * Time.deltaTime);
            visual.transform.RotateAround(transform.forward, visualRotationPerSecond.z * Time.deltaTime);
        }

        // Check collision
        if (CollisionCheck())
            StopProjectile();

        lastPosition = transform.position;
    }

    private void StopProjectile()
    {
        stopped = true;

        // Set correct position and hierarchy
        transform.position = hitDetected.point + transform.forward * penetration;
        transform.SetParent(hitDetected.transform);
        
        // Activate pickup object
        if(pickupable && pickup)
            pickup.SetActive(true);
        
        // Enable stationary collider
        if(collider)
            collider.enabled = true;

        // Turn off hitboxes
        for(int i=0; i < hitbox.Length; i++)
        {
            if(hitbox[i].GetColliding())
                hitbox[i].StopColliding();
        }
    }

    // Make sure we don't fly through colliders due to speed
    private bool CollisionCheck()
    {   
        if(lastPositionCheck)
            return Physics.Raycast(lastPosition, transform.position, out hitDetected, speed * 2f * Time.deltaTime, hitMask);
        else
            return Physics.Raycast(transform.position, transform.forward, out hitDetected, speed * 2f * Time.deltaTime, hitMask);
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox)
        {
            // Avoid triggering multiple hurtboxes with the same parent GameObject
            if(objectsCollided.Contains(hurtbox.transform.parent.gameObject) == false)
            {
                hurtbox.Hit(softDamage, hardDamage, DamageType.Pierce);
                objectsCollided.Add(hurtbox.transform.parent.gameObject);
            }        
        }
    }

    public void UpdateColliderState(bool state)
    {
        if(state == false)
            objectsCollided.Clear();
    }
}
