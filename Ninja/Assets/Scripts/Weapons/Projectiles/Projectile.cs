using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IHitboxResponder
{   
    [Header("Damage")]
    [SerializeField] private AttackInfo attackInfo;
    

    [Space (10)]

    [Header("Force")]
    [SerializeField] private float pushForce = 5;

    [Space (10)]

    [Header("Gravity")]
    [SerializeField] private bool gravity;
    [SerializeField] private bool useGlobalGravity;
    [SerializeField] private Vector3 customGravity;

    [Space (10)]

    [Header("Visual")]
    [SerializeField] private Vector3 visualRotationPerSecond;
    [SerializeField] private Vector3 parentRotationPerSecond;
    [SerializeField] private GameObject visual;
    [SerializeField] private Vector3 minRotation;
    [SerializeField] private Vector3 maxRotation;
    [SerializeField] private float penetration = -.1f;

    [Space (10)]

    [Header("Trajectory")]
    [SerializeField] private float speed;
    [SerializeField] private bool stopped;

    [SerializeField] private bool lastPositionCheck;
    [SerializeField] private Vector3 lastPosition;
    [SerializeField] private RaycastHit hitDetected;
    [SerializeField] private LayerMask hitMask;

    [Space (10)]

    [Header("Hitbox")]
    [SerializeField] private Hitbox[] hitbox;
    private bool stoppedHitbox;

    [Space (10)]

    [Header ("Collider")]
    [Tooltip("Will enable this collider when stopped")]
    [SerializeField] private new Collider collider;

    [Space (10)]

    [Tooltip("Activate when stopped")]
    [SerializeField] private float activateDelay = 0;
    [SerializeField] private GameObject[] objectsToActivate;

    [Space (10)]

    [Header ("Deactivate when stopped")]
    [SerializeField] private float deactivateDelay = 0;
    [SerializeField] private GameObject[] objectsToDeactivate;

    [Space (10)]

    [Header ("Pickup Reference")]
    [SerializeField] private GameObject pickup;

    [Header ("Real Time Data")]
    [SerializeField] private List<GameObject> objectsCollided = new List<GameObject>();
    [SerializeField] private Transform ignoreTransform;

    private float startTime;
    private Vector3 startPos;
    private Vector3 startSpeed;
    private StealthAgent stealthAgent;

    public delegate void ProjecitleStopDelegate(RaycastHit hit);
    public event ProjecitleStopDelegate OnProjecitleStop;

    private void Start()
    {
        Restart();

        for(int i=0; i < hitbox.Length; i++)
        {
            hitbox[i].SetResponder(this);
            hitbox[i].StartColliding();
        }

    }

    public void Restart()
    {
        startTime = Time.time;
        startPos = transform.position;
        startSpeed = transform.forward * speed;
        lastPosition = transform.position;
    }

    public void SetIgnoreTransform(Transform transform)
    {
        ignoreTransform = transform;
        foreach(Hitbox h in hitbox)
            h.SetIgnoreTransform(transform);
    }

    public void SetOwner(StealthAgent agent)
    {
        stealthAgent = agent;
    }

    private void Update()
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
            StopProjectile(hitDetected);

        lastPosition = transform.position;
    }

    public void StopProjectile(RaycastHit hit)
    {
        stopped = true;

        // Set correct position and hierarchy
        transform.position = hit.point + transform.forward * penetration;
        transform.SetParent(hit.transform, true);
        
        // Apply physics
        Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
        if(rb == null)
            rb = hit.transform.GetComponentInParent<Rigidbody>();
        if(rb)
            rb.AddForce(transform.forward * pushForce, ForceMode.Impulse);

        // Enable stationary collider
        if(collider)
            collider.enabled = true;
        
        // Activate objects
        StartCoroutine(SetObjectsActiveDelay(objectsToActivate, true, activateDelay));

        // Deactivate objects
        StartCoroutine(SetObjectsActiveDelay(objectsToDeactivate, false, deactivateDelay));

        // Notify listeners
        if(OnProjecitleStop != null)
            OnProjecitleStop(hit);
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
        bool hit;

        if(lastPositionCheck)
            hit = Physics.Raycast(lastPosition, transform.position - lastPosition, out hitDetected, speed * 2f * Time.deltaTime, hitMask);
        else
            hit = Physics.Raycast(transform.position, transform.forward, out hitDetected, speed * 2f * Time.deltaTime, hitMask);

        if(hit && hitDetected.transform.root == ignoreTransform)
            hit = false;

        return hit;
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox)
        {
            // Avoid triggering multiple hurtboxes with the same parent GameObject
            if(objectsCollided.Contains(hurtbox.transform.root.gameObject) == false)
            {
                hurtbox.Hit(stealthAgent, attackInfo, Vector3.up, transform.forward * pushForce);
                objectsCollided.Add(hurtbox.transform.root.gameObject);
            }        
        }
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
    
    public void UpdateColliderState(bool state)
    {
        if(state == false)
            objectsCollided.Clear();
    }

    public GameObject ReplaceWithPickup()
    {
        GameObject go = Instantiate(pickup, transform.position, transform.rotation, transform.parent);
        go.transform.localScale = transform.localScale;
        Destroy(gameObject);
        return go;
    }
}
