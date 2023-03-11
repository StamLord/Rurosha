using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private enum HitboxShape {BOX, SPHERE}

    [Header("Hitbox Settings")]
    [SerializeField] private HitboxShape shape;
    [SerializeField] private Vector3 size = new Vector3(1,1,1);
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private bool hitMultipleHurtbox = true;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private LayerMask perfectGuardMask;
    [SerializeField] private LayerMask guardMask;
    [SerializeField] public bool isActive;

    [Header("Velocity")]
    [SerializeField] private Vector3 lastPosition;
    [SerializeField] private Vector3 velocity;
    
    [SerializeField] private bool overrideVelocity;
    [SerializeField] private Transform oVelocityReference;
    [SerializeField] private Vector3 oVelocity;
    public Vector3 Velocity 
    {get {
        if(overrideVelocity)
        {
            if(oVelocityReference)
                return oVelocityReference.TransformVector(oVelocity);
            else
                return transform.TransformDirection(oVelocity);
        }
        return velocity;
    }}
    
    [Header("Debug Info")]
    [SerializeField] private bool lastActiveState;
    [SerializeField] private IHitboxResponder _responder;

    [SerializeField] private List<Collider> collided = new List<Collider>();

    private bool activeForOneFrame = false;
    private Transform ignoreTransform;

    public Vector3 Position { get { return transform.position + offset;}}

    public void StartColliding(bool activeForOneFrame = false)
    {
        isActive = true;    
        this.activeForOneFrame = activeForOneFrame;
        lastPosition = transform.position + offset;
    }

    public void StopColliding()
    {
        isActive = false;
    }

    public void ForgetCollided()
    {
        collided.Clear();
    }

    public bool GetColliding()
    {
        return isActive;
    }

    public void SetResponder(IHitboxResponder responder)
    {
        _responder = responder;
    }

    public void SetIgnoreTransform(Transform transform)
    {
        ignoreTransform = transform;
    }

    private void Update()
    {
        if(lastActiveState != isActive)
        {    
            if(_responder != null) _responder.UpdateColliderState(isActive);
            if(isActive == false)
                collided.Clear();
        }

        if(isActive)
        {
            Vector3 centerOffset = transform.position + transform.TransformVector(offset);
            Vector3 sizeScaled = new Vector3(transform.lossyScale.x * size.x, transform.lossyScale.y * size.y, transform.lossyScale.z * size.z);

            // Check if hitbox hits a perfect guard collider
            Collider[] perfectGuardColliders = GetColliders(perfectGuardMask);

            // If hitbox guarded by anything we inform responder and don't proceed
            if(perfectGuardColliders.Length > 0)
            {
                _responder?.PerfectGuardedBy(perfectGuardColliders[0], this);
                StopColliding();
                collided.Clear();
                return;
            }

            // Check if hitbox hits a guard collider
            Collider[] guardColliders = GetColliders(guardMask);

            // If hitbox guarded by anything we inform responder and don't proceed
            if(guardColliders.Length > 0)
            {
                _responder?.GuardedBy(guardColliders[0], this);
                return;
            }
            
            // Get hits in range
            Collider[] colliders = GetColliders(hitMask);

            foreach(Collider col in colliders)
            {
                // Skip ignore
                if(col.transform.root == ignoreTransform)
                    continue;
                
                // Check if we didn't already collide with it to avoid multiple detections
                if(collided.Contains(col) == false)
                {
                    // If we should not hit more than one hurtbox per target, check all collided so far
                    if(hitMultipleHurtbox == false)
                    {
                        foreach(Collider c in collided)
                        {
                            if(c.transform.root == col.transform.root )
                                return;
                        }
                    }

                    // Perform collision
                    _responder?.CollisionWith(col, this);
                    collided.Add(col);
                }
            }

            if(activeForOneFrame)
            {
                StopColliding();
                collided.Clear();
            }
        }

        lastActiveState = isActive;
        
        // Calculate Velocity
        velocity = transform.position + offset - lastPosition;
        // Track position for next frame velocity calculations
        lastPosition = transform.position + offset;
    }

    private Collider[] GetColliders(LayerMask mask)
    {
        Collider[] colliders = new Collider[0];

        Vector3 centerOffset = transform.position + transform.TransformVector(offset);
        Vector3 sizeScaled = new Vector3(transform.lossyScale.x * size.x, transform.lossyScale.y * size.y, transform.lossyScale.z * size.z);

        switch(shape)
        {
            case HitboxShape.BOX:
                colliders = Physics.OverlapBox(centerOffset, sizeScaled *.5f, transform.rotation, mask);
                break;
            case HitboxShape.SPHERE:
                colliders = Physics.OverlapSphere(centerOffset, sizeScaled.x, mask);
                break;
        }

        return colliders;
    }

    private void OnDrawGizmos()
    {
        Color red = Color.red;
        red.a = (isActive) ? .75f : .25f;
        Gizmos.color = red;

        // Draw Velocity
        // Needs to be drawn before gizmos matrix is localized
        Gizmos.DrawRay(offset, Velocity);

        // Draw hitbox
        Gizmos.matrix = this.transform.localToWorldMatrix;

        switch(shape)
        {
            case HitboxShape.BOX:
                Gizmos.DrawCube(offset, size);
                break;
            case HitboxShape.SPHERE:
                Gizmos.DrawSphere(offset, size.x);
                break;
        }
        
    }
}
