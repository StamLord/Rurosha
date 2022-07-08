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
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private LayerMask guardMask;
    [SerializeField] private bool isActive;

    public Vector3 Position { get { return transform.position + offset;}}
    
    [Header("Debug Info")]
    [SerializeField] private bool lastActiveState;
    [SerializeField] private IHitboxResponder _responder;

    [SerializeField] private List<Collider> collided = new List<Collider>();

    private bool activeForOneFrame = false;

    [SerializeField] private Vector3 lastPosition;
    [SerializeField] private Vector3 velocity;
    public Vector3 Velocity {get {return velocity;}}

    private Transform ignoreTransform;

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

            // Check if hitbox hits a guard collider
            Collider[] guardColliders = new Collider[0];
            switch(shape)
            {
                case HitboxShape.BOX:
                    guardColliders = Physics.OverlapBox(centerOffset, sizeScaled *.5f, transform.rotation, guardMask);
                    break;
                case HitboxShape.SPHERE:
                    guardColliders = Physics.OverlapSphere(centerOffset, sizeScaled.x, guardMask);
                    break;
            }

            // If hitbox guarded by anything we inform responder and don't proceed
            if(guardColliders.Length > 0)
            {
                _responder.GuardedBy(guardColliders[0], this);
                StopColliding();
                collided.Clear();
                return;
            }
            
            // Get colliders in range
            Collider[] colliders = new Collider[0];

            switch(shape)
            {
                case HitboxShape.BOX:
                    colliders = Physics.OverlapBox(centerOffset, sizeScaled *.5f, transform.rotation, hitMask);
                    break;
                case HitboxShape.SPHERE:
                    colliders = Physics.OverlapSphere(centerOffset, sizeScaled.x, hitMask);
                    break;
            }

            foreach(Collider col in colliders)
            {
                // Skip ignore
                if(col.transform.root == ignoreTransform)
                    continue;
                
                // Collide if we didn't already collide with it
                if(collided.Contains(col) == false)
                {
                    _responder.CollisionWith(col, this);
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

    private void OnDrawGizmos()
    {
        Color red = Color.red;
        red.a = (isActive) ? .75f : .25f;
        Gizmos.color = red;
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
