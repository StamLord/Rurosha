using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public enum CarryType {FIXED, PHYSICS, JOINT};

    [Header("References")]
    [SerializeField] private new Transform camera;
    [SerializeField] private Transform carryPoint;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private CharacterStats characterStats;
    public WeaponManager weaponManager { get {return _weaponManager;} }

    [Space(10)]

    [Header("Interaction")]
    [SerializeField] private float interactionRange;
    [SerializeField] private LayerMask interactionMask;

    [SerializeField] private Usable currentSelection;

    public delegate void SelectionChangeDelegate(string selectionText);
    public event SelectionChangeDelegate SelectionChangeEvent;

    [Space(10)]

    [Header("Carry")]

    [SerializeField] private float startPressTime;
    [SerializeField] private float minTimeToCarry = 2f;

    [Space(10)]

    [SerializeField] private bool attemptingCarry;
    [SerializeField] private bool isCarrying;
    [SerializeField] private Rigidbody carriedObject;

    [Space(10)]

    [SerializeField] private CarryType carryType;
    
    [Space(10)]

    [SerializeField] private float throwForce;
    
    [Header("Physics Settings")]
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float drag = 10f;
    [SerializeField] private float angularDrag = 20f;

    private float oldDrag;
    private float oldAngularDrag;
    
    [Header("Joint Settings")]
    [SerializeField] private bool isSpring;
    [SerializeField] private HingeJoint joint;
    [SerializeField] private Rigidbody springRigid;
    [SerializeField] private float spring = 50f;
    [SerializeField] private float damper = 5f;
    [SerializeField] private float maxDistance = .2f;

    [SerializeField] private bool debugView;

    public delegate void updateCarryTimerDelegate(float time);
    public event updateCarryTimerDelegate UpdateCarryTimerEvent;

    void Start()
    {
        InitializeCarry();        
    }

    void InitializeCarry()
    {
        switch(carryType)
        {
            case CarryType.JOINT:
                if (!joint)
                {
                    GameObject go = new GameObject("Carry");
                    springRigid = go.AddComponent<Rigidbody>();
                    joint = go.AddComponent<HingeJoint>();
                    joint.autoConfigureConnectedAnchor = false;
                    joint.axis = new Vector3(0,1,0);
                    springRigid.isKinematic = true;

                    if(isSpring)
                    {
                        JointSpring s = new JointSpring();
                        s.spring = spring;
                        s.damper = damper;
                        //s.maxDistance = maxDistance;
                        joint.spring = s;
                    }
                    
                    joint.transform.SetParent(carryPoint);
                    joint.transform.localPosition = Vector3.zero;
                }
                break;
        }
    }

    void Update()
    {
        CheckObject();

        float carryTimer = Time.time - startPressTime;

        if(UpdateCarryTimerEvent != null)
            UpdateCarryTimerEvent((attemptingCarry)? carryTimer / minTimeToCarry : 0f);

        if(Input.GetKeyDown(KeyCode.E))
        {
            startPressTime = Time.time;
            attemptingCarry = true;
        }
        else if(Input.GetKeyUp(KeyCode.E))
        {
            attemptingCarry = false;
            UpdateCarryTimerEvent(0);

            if(carryTimer <  minTimeToCarry && 
                isCarrying == false)
                currentSelection?.Use(this);
            else if(isCarrying)
                StopCarry();
            else
                StartCarry();
        }

        if(Input.GetButton("Fire1"))
        {
            if(isCarrying) 
            {
                Rigidbody co = carriedObject;
                StopCarry();
                co.AddForce(camera.forward * throwForce, ForceMode.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        UpdateCarriedObject();
    }
    
    void CheckObject()
    {
        //Debug.DrawRay(camera.position, camera.forward * interactionRange);
        RaycastHit hit;
        if(Physics.Raycast(camera.position, camera.forward, out hit, interactionRange, interactionMask))
        {
            Usable usable = hit.transform.GetComponent<Usable>();
            if(usable) 
            {
                ChangeSelection(usable);
                return;
            }
        }

        ClearSelection();
    }

    void ClearSelection()
    {
        currentSelection?.Highlight(false);
        currentSelection = null;

        if(SelectionChangeEvent != null)
            SelectionChangeEvent("");
    }

    void ChangeSelection(Usable usable)
    {
        if(currentSelection) ClearSelection();
        usable.Highlight(true);
        currentSelection = usable;

        if(SelectionChangeEvent != null)
            SelectionChangeEvent(currentSelection.GetText());
    }

    void StartCarry()
    {
        PhysicalObject po = currentSelection.GetComponent<PhysicalObject>();
        if(po == null || characterStats.CanPickup(po.GetWeight()) == false)
            return;

        isCarrying = true;
        carriedObject = currentSelection.transform.GetComponent<Rigidbody>();
        
        switch(carryType)
        {
            case CarryType.FIXED:

                carriedObject.isKinematic = true;
                carriedObject.freezeRotation = true;
                carriedObject.transform.SetParent(carryPoint);

                break;
            case CarryType.PHYSICS:

                carriedObject.useGravity = false;

                oldDrag = carriedObject.drag;
                oldAngularDrag = carriedObject.angularDrag;

                carriedObject.drag = drag;
                carriedObject.angularDrag = angularDrag;

                carriedObject.transform.parent = carryPoint;
                
                break;
            case CarryType.JOINT:

                if(joint)
                {
                    //springJoint.transform.position = springJoint.transform.InverseTransformPoint(carriedObject.position);
                    carriedObject.freezeRotation = true;
                    joint.connectedAnchor = joint.transform.InverseTransformPoint(carriedObject.position);
                    joint.connectedBody = carriedObject;
                }

                break;
        }
    }

    void StopCarry()
    {
        switch(carryType)
        {
            case CarryType.FIXED:

                carriedObject.isKinematic = false;
                carriedObject.freezeRotation = false;
                carriedObject.transform.SetParent(null);
                break;

            case CarryType.PHYSICS:

                carriedObject.useGravity = true;
                carriedObject.drag = oldDrag;
                carriedObject.angularDrag = oldAngularDrag;
                carriedObject.transform.parent = null;

                break;

            case CarryType.JOINT:
                if(joint)
                {    
                    joint.connectedBody = null;
                    carriedObject.freezeRotation = false;
                }
                break;
        }
                
        isCarrying = false;
        carriedObject = null;
    }

    void UpdateCarriedObject()
    {
        if(isCarrying == false || carryType != CarryType.PHYSICS)
            return;

        if(Vector3.Distance(carriedObject.position, carryPoint.position) > .05f)
        {
            Vector3 moveDir = carryPoint.position - carriedObject.position;
            carriedObject.AddForce(moveDir * moveForce);
        }
    }

    void OnDrawGizmos()
    {
        if(!debugView) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(camera.position, camera.forward * interactionRange);

        if(!joint) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(joint.transform.position, .05f);
    }
}