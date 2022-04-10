using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Vector3 size = new Vector3(1,1,1);
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private bool isActive;
    [SerializeField] private bool lastActiveState;
    [SerializeField] private IHitboxResponder _responder;

    [SerializeField] private List<Collider> collided = new List<Collider>();

    private bool activeForOneFrame = false;

    public void StartColliding(bool activeForOneFrame = false)
    {
        isActive = true;    
        this.activeForOneFrame = activeForOneFrame;
    }

    public void StopColliding()
    {
        isActive = false;
    }

    public bool GetColliding()
    {
        return isActive;
    }

    public void SetResponder(IHitboxResponder responder)
    {
        _responder = responder;
    }

    void Update()
    {
        if(lastActiveState != isActive)
        {    
            _responder.UpdateColliderState(isActive);
            if(isActive == false)
                collided.Clear();
        }

        if(isActive)
        {
            Vector3 centerOffset = transform.position + transform.TransformVector(offset);
            Vector3 sizeScaled = new Vector3(transform.lossyScale.x * size.x, transform.lossyScale.y * size.y, transform.lossyScale.z * size.z);
            
            Collider[] colliders = Physics.OverlapBox(centerOffset, sizeScaled / 2, transform.rotation, hitMask);

            // Debug.DrawLine(transform.position, transform.position + transform.TransformVector(offset), Color.red, 1f);

            // Debug.DrawLine(centerOffset, centerOffset + transform.right * sizeScaled.x / 2, Color.yellow, 1f);
            // Debug.DrawLine(centerOffset, centerOffset + transform.up * sizeScaled.y / 2, Color.yellow, 1f);
            // Debug.DrawLine(centerOffset, centerOffset + transform.forward * sizeScaled.z / 2, Color.yellow, 1f);
            // Debug.Break();

            foreach(Collider col in colliders)
            {
                if(collided.Contains(col) == false)
                {
                    _responder.CollisionWith(col, this);
                    collided.Add(col);
                }
            }

            if(activeForOneFrame)
                StopColliding();
        }

        lastActiveState = isActive;
    }

    void OnDrawGizmos()
    {
        Color red = Color.red;

        red.a = (isActive) ? .75f : .25f;
        Gizmos.color = red;

        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.DrawCube(offset, size);
    }
}
