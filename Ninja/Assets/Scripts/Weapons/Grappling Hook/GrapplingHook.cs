using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : WeaponObject
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private CharacterStateMachine characterStateMachine;
    [SerializeField] private GrappleState grappleState;
    [SerializeField] private GameObject projectile;

    [Header("Spring Settings")]
    [SerializeField] private float springForce = 100;
    [SerializeField] private float springDamper = 10;

    [Header("Grapple Settings")]
    [SerializeField] private Transform grappleOrigin;
    [SerializeField] private float pullRate = 2f;
    [SerializeField] private float rigidPullForce = 10f;
    [SerializeField] private float maxFlyTime = 1f;
    [SerializeField] private float pickupPullSpeed = 1f;
    [SerializeField] private float pickupDistance = .2f;
    private float fireStartTime;
    private bool pickupPullStarted;

    [Header("Animation")]
    [SerializeField] private Vector3 ropeProjectileOffset;
    [SerializeField] private int animationPoints = 10;
    [SerializeField] private int animationWaves = 5;
    [SerializeField] private float animationWaveHeight = .2f;
    [SerializeField] private AnimationCurve animationWaveHeightOverTime;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private ParticleSystem pullVfx;
    [SerializeField] private GameObject spinningCylinder;

    [Header("Real Time Data")]
    [SerializeField] private State state;
    [SerializeField] private Vector3 grapplePoint;
    [SerializeField] private Transform grappleTo;
    [SerializeField] private Rigidbody rigidGrappleTo;
    [SerializeField] private Pickup pickupGrappleTo;
    [SerializeField] private SpringJoint joint;
    [SerializeField] private Projectile projInstance;

    private enum State {READY, FIRING, GRAPPLED}

    void Awake()
    {
        player = transform.root.gameObject;
    }

    void Update()
    {
        ProcessInput();
        DrawRope();
        SpinCylinder();
    }

    private void ProcessInput()
    {
        switch(state)
        {
            case State.READY:
                if(inputState.MouseButton1.State == VButtonState.PRESS_START)
                    StartFire();
                break;
            case State.FIRING:
                if(Time.time - fireStartTime >= maxFlyTime)
                    StopGrapple();
                break;
            case State.GRAPPLED:
                // Cancel grapple
                if(inputState.MouseButton2.State == VButtonState.PRESS_START)
                    StopGrapple();
                else if(inputState.MouseButton1.State == VButtonState.PRESS_START)
                {
                    SpinCylinder(360f * Time.deltaTime);
                    // Pull pickup to you and add item
                    if(pickupGrappleTo && pickupPullStarted == false)
                        StartCoroutine("PullPickup");
                    // Pull rigidbody to you
                    else if(rigidGrappleTo)
                    {
                        if(pullVfx.isStopped) pullVfx.Play();
                        rigidGrappleTo.AddForce((grappleOrigin.position - rigidGrappleTo.position) * rigidPullForce, ForceMode.VelocityChange);
                    }
                }
                // Slowly pull yourself to target
                else if(inputState.MouseButton1.State == VButtonState.PRESSED)
                {
                    joint.maxDistance -= Time.deltaTime * pullRate;
                    if(pullVfx.isStopped) pullVfx.Play();
                    SpinCylinder(360f * Time.deltaTime);
                }
                else
                    pullVfx.Stop();
                break;
        }
    }

    private void StartFire()
    {
        state = State.FIRING;
        fireStartTime = Time.time;

        // Get direction of projectile from hand to target
        RaycastHit hit;
        Vector3 direction;
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 50f))
            direction = hit.point - grappleOrigin.position;
        else
            direction = camera.transform.position + camera.transform.forward * 50 - grappleOrigin.position;

        // Instantiate and orient projectile
        projInstance = Instantiate(projectile, grappleOrigin.position, Quaternion.identity).GetComponent<Projectile>();
        projInstance.transform.forward = direction;
        
        // Prepare projectile
        projInstance.SetIgnoreTransform(transform.root);
        projInstance.OnProjecitleStop += StartGrapple;
    }

    private void StartGrapple(RaycastHit hit)
    {
        state = State.GRAPPLED;
        grappleTo = hit.transform;
        grapplePoint = hit.point;
        
        // Create joint component
        joint = player.AddComponent<SpringJoint>();
        joint.axis = new Vector3(1,1,1);
        joint.autoConfigureConnectedAnchor = false;

        float distanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint);
        joint.maxDistance = distanceFromPoint; // * .8f;
        joint.minDistance = 0f; //distanceFromPoint * .25f;
        joint.spring = springForce;
        joint.damper = springDamper;

        // Get Rigidbody
        rigidGrappleTo = hit.collider.GetComponent<Rigidbody>();

        // Get Pickup
        pickupGrappleTo = hit.collider.GetComponent<Pickup>();
        
        if(rigidGrappleTo)
        {
            joint.connectedBody = rigidGrappleTo;
            joint.massScale = 0; // We want only the grappled object to move
        }
        else
        {
            joint.connectedAnchor = grapplePoint;
            joint.massScale = 4.5f;
            characterStateMachine.SwitchState(8); // Grapple State
        }
    }

    private void StopGrapple()
    {
        state = State.READY;
        pullVfx.Stop();
        
        if(pickupPullStarted)
            StopCoroutine("PullPickup");

        // Destory join component
        Destroy(joint);

        // Unsubscribe and destroy projectile
        projInstance.OnProjecitleStop -= StartGrapple;
        Destroy(projInstance.gameObject);

        // Switch character to Air State
        characterStateMachine.SwitchState(5);
    }

    private void DrawRope()
    {
        switch(state)
        {
            case State.READY:
                if(lr.enabled) lr.enabled = false;
                break;
            case State.FIRING:
                if(lr.enabled == false) 
                    lr.enabled = true;
                
                lr.positionCount = animationPoints;

                for (int i = 0; i < animationPoints; i++)
                {
                    float p = (float)i / animationPoints;
                    Vector3 pos = Vector3.Lerp(grappleOrigin.position, projInstance.transform.position + projInstance.transform.TransformVector(ropeProjectileOffset), p);
                    pos += projInstance.transform.up * Mathf.Sin(p * animationWaves * Mathf.PI) * animationWaveHeight * animationCurve.Evaluate(p) * animationWaveHeightOverTime.Evaluate((Time.time - fireStartTime) / maxFlyTime);
                    lr.SetPosition(i, pos);
                }

                break;
            case State.GRAPPLED:
                if(lr.enabled == false) 
                    lr.enabled = true;
                lr.positionCount = 2;
                lr.SetPosition(0, grappleOrigin.position);
                lr.SetPosition(1, projInstance.transform.position + projInstance.transform.TransformVector(ropeProjectileOffset));
                break;
        }
    }
    
    private IEnumerator PullPickup()
    {
        pickupPullStarted = true;

        if(rigidGrappleTo)
            rigidGrappleTo.isKinematic = true;
        
        while(Vector3.Distance(pickupGrappleTo.transform.position, transform.position) > pickupDistance)
        {
            pickupGrappleTo.transform.position += (transform.position - pickupGrappleTo.transform.position).normalized * pickupPullSpeed * Time.deltaTime;
            yield return null;
        }

        pickupGrappleTo.Use(manager);
        StopGrapple();

        if(rigidGrappleTo)
            rigidGrappleTo.isKinematic = false;

        pickupPullStarted = false;
    }

    private void SpinCylinder()
    {
        switch(state)
        {
            case State.FIRING:
                SpinCylinder(-720 * Time.deltaTime);
                break;
        }
    }

    private void SpinCylinder(float amount)
    {
        spinningCylinder.transform.Rotate(new Vector3(amount, 0, 0));
    }
}
