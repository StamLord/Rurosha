using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IHitboxResponder
{   
    [Header("Damage")]
    [SerializeField] private int softDamage = 5;
    [SerializeField] private int hardDamage = 5;

    [Header("Force")]
    [SerializeField] private float pushForce = 5;

    [Header("Gravity")]
    [SerializeField] private bool gravity;
    [SerializeField] private bool useGlobalGravity;
    [SerializeField] private Vector3 customGravity;

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
    [SerializeField] private LayerMask hitMask;

    private float startTime;
    private Vector3 startPos;
    private Vector3 startSpeed;

    [Header("Hitbox")]
    [SerializeField] private Hitbox[] hitbox;
    private bool stoppedHitbox;

    [Header ("Collider")]
    [Tooltip("Will enable this collider when stopped")]
    [SerializeField] private new Collider collider;

    [Tooltip("Activate when stopped")]
    [SerializeField] private float activateDelay = 0;
    [SerializeField] private GameObject[] objectsToActivate;

    [Header ("Deactivate when stopped")]
    [SerializeField] private float deactivateDelay = 0;
    [SerializeField] private GameObject[] objectsToDeactivate;

    [SerializeField] private List<GameObject> objectsCollided = new List<GameObject>();

    void Start()
    {
        startTime = Time.time;
        startPos = transform.position;
        startSpeed = transform.forward * speed;

        lastPosition = transform.position;

        for(int i=0; i < hitbox.Length; i++)
        {
            hitbox[i].SetResponder(this);
            hitbox[i].StartColliding();
        }

    }

    void Update()
    {
        // If we stopped, turn off hitbox and don't process anything else
        if(stopped && stoppedHitbox)
            return;
        else if(stopped)
        {   
            // We stop hitbox seperately from stopping projectile so hitbox stays active for 1 more frame and we ensure a hit
            StopHitbox();
            return;
        }
        
        // Calculate position
        float timePassed = Time.time - startTime;
        Vector3 newPos = startPos + startSpeed * timePassed;

        // Gravity
        if(gravity)
            if(useGlobalGravity)
                newPos.y = startPos.y + startSpeed.y * timePassed + Physics.gravity.y *.5f * timePassed * timePassed;
            else
                newPos.y = startPos.y + startSpeed.y * timePassed + customGravity.y *.5f * timePassed * timePassed;
        
        // Set new position
        transform.position = newPos;
        // Rotate in direction of flight
        transform.forward = transform.position - lastPosition;

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
        transform.SetParent(hitDetected.transform, true);
        
        // Apply physics
        Rigidbody rb = hitDetected.transform.GetComponent<Rigidbody>();
        if(rb == null)
            rb = hitDetected.transform.GetComponentInParent<Rigidbody>();
        if(rb)
            rb.AddForce(transform.forward * pushForce, ForceMode.Impulse);

        // Enable stationary collider
        if(collider)
            collider.enabled = true;

        // Activate objects
        SetObjectsActiveDelay(objectsToActivate, true, activateDelay);

        // Deactivate objects
        SetObjectsActiveDelay(objectsToDeactivate, false, deactivateDelay);
    }

    private void StopHitbox()
    {
        // Turn off hitboxes
        for(int i=0; i < hitbox.Length; i++)
        {
            if(hitbox[i].GetColliding())
                hitbox[i].StopColliding();
        }

        stoppedHitbox = true;
    }

    private IEnumerator SetObjectsActiveDelay(GameObject[] objects, bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetObjectsActive(objects, state);
    }

    private void SetObjectsActive(GameObject[] objects, bool state)
    {
        for(int i = 0; i < objects.Length; i++)
            objects[i].SetActive(state);
    }

    // Make sure we don't fly through colliders due to speed
    private bool CollisionCheck()
    {   
        if(lastPositionCheck)
            return Physics.Raycast(lastPosition, transform.position - lastPosition, out hitDetected, speed * 2f * Time.deltaTime, hitMask);
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
                objectsCollided.Add(hurtbox.transform.root.gameObject);
            }        
        }
    }

    public void UpdateColliderState(bool state)
    {
        if(state == false)
            objectsCollided.Clear();
    }
}
