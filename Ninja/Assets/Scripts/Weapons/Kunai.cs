using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kunai : WeaponObject, IHitboxResponder
{
    [SerializeField] private GameObject player;
    [SerializeField] private new Rigidbody rigidPlayer;

    [SerializeField] private GameObject staticKunai;
    [SerializeField] private GameObject dynamicKunai;

    [SerializeField] private HingeJoint joint;
    [SerializeField] private bool isHanging;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Hitbox hitbox;

    [SerializeField] private Vector3 anchor = new Vector3(0,1,0);
    [SerializeField] private Vector3 ConnectedAnchor = new Vector3(0,1.5f,0);

    // [SerializeField] private float spring = 30f;
    // [SerializeField] private float damper = 10f;
    // [SerializeField] private float massScale = 4.5f;
    // [SerializeField] private float minDistance = .05f;
    // [SerializeField] private float maxDistance = .1f;

    void Awake()
    {
        base.Initialize();
        rigidbody = transform.parent.parent.GetComponent<Rigidbody>();
        hitbox?.SetResponder(this);
        player = transform.parent.parent.gameObject;
        rigidPlayer = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {   
            isHanging = true;
            AltUseAnimation();
        }

        if(Input.GetMouseButtonUp(1) && isHanging ||
            Input.GetMouseButton(1) == false && isHanging)
            StopHanging();

        animator.SetBool("IsHanging", isHanging);
    }

    void StartHanging()
    {
        
        dynamicKunai.SetActive(false);
        staticKunai.SetActive(true);
        staticKunai.transform.position = dynamicKunai.transform.position;
        staticKunai.transform.rotation = dynamicKunai.transform.rotation;
        staticKunai.transform.SetParent(null);
    }

    void StopHanging()
    {
        isHanging = false;
        
        dynamicKunai.SetActive(true);
        staticKunai.SetActive(false);
        staticKunai.transform.SetParent(transform);

        if(joint)
        {
            Destroy(joint);
            rigidbody.rotation = Quaternion.Euler(0, rigidbody.rotation.eulerAngles.y, 0);
            rigidPlayer.freezeRotation = true;
        }
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        if(joint) return;
        
        //Vector3 hangPoint = hitbox.transform.position;

        joint = player.AddComponent<HingeJoint>();
        joint.axis = new Vector3(1,0,1);
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = transform.position + ConnectedAnchor;
        joint.anchor = anchor;
        joint.enablePreprocessing = false;

        joint.useSpring = true;
        JointSpring spring = new JointSpring();
        spring.damper = 1f;

        joint.spring = spring;

    // float distanceFromPoint = Vector3.Distance(player.transform.position, hangPoint);
        // joint.maxDistance = distanceFromPoint * maxDistance;
        // joint.minDistance = distanceFromPoint * minDistance;

        // joint.spring = spring;
        // joint.damper = damper;
        // joint.massScale = massScale;

        rigidPlayer.freezeRotation = false;

        StartHanging();
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

    public void UpdateColliderState(bool newState)
    {

    }
}
