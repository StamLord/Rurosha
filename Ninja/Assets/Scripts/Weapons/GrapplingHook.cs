using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SpringJoint joint;
    [SerializeField] private LineRenderer lr;
    
    [SerializeField] private LayerMask grappleMask;

    [SerializeField] private Transform grappleOrigin;
    [SerializeField] private Transform grappleTo;
    [SerializeField] private Vector3 grapplePoint;

    [SerializeField] private bool started;
    [SerializeField] private float maxDistance;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        player = transform.parent.parent.gameObject;
    }

    void Update()
    {
        if(started == false && Input.GetMouseButtonDown(0))
            StartGrapple();

        if(started && Input.GetMouseButtonDown(1))
            StopGrapple();

        if(started)
        {
            if(lr.enabled == false) lr.enabled = true;

            lr.SetPosition(0, grappleOrigin.position);
            lr.SetPosition(1, grapplePoint);
        }
        else
        {
            if(lr.enabled) lr.enabled = false;
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, grappleMask))
        {
            started = true;
            grappleTo = hit.transform;
            grapplePoint = hit.point;
            
            joint = player.AddComponent<SpringJoint>();
            joint.axis = new Vector3(1,1,1);
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * .8f;
            joint.minDistance = distanceFromPoint * .25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
            

        }
    }

    void StopGrapple()
    {
        started = false;
        Destroy(joint);
    }

}
