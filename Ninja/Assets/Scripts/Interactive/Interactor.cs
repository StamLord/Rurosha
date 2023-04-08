using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public enum CarryType {FIXED, PHYSICS, JOINT};

    [Header("References")]
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _carryPoint;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private InputState _inputState;
    [SerializeField] private CharacterStats _characterStats;

    [Space(10)]

    [Header("Interaction")]
    [SerializeField] private float _interactionRange = 2f;
    [SerializeField] private LayerMask _interactionMask;

    [SerializeField] private Usable _currentSelection;

    public delegate void SelectionChangeDelegate(string selectionText);
    public event SelectionChangeDelegate SelectionChangeEvent;

    [Space(10)]

    [Header("Carry")]

    [SerializeField] private float _startPressTime;
    [SerializeField] private float _minTimeToCarry = 1f;

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

    private void Start()
    {
        InitializeCarry();   
    }

    private void InitializeCarry()
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
                    
                    joint.transform.SetParent(_carryPoint);
                    joint.transform.localPosition = Vector3.zero;
                }
                break;
        }
    }

    private void Update()
    {
        CheckObject();

        float carryTimer = Time.time - _startPressTime;

        if(UpdateCarryTimerEvent != null)
            UpdateCarryTimerEvent((attemptingCarry)? carryTimer / _minTimeToCarry : 0f);

        if(_inputState.Use.State == VButtonState.PRESS_START)
        {
            _startPressTime = Time.time;
            attemptingCarry = true;
        }
        else if(_inputState.Use.State == VButtonState.PRESS_END)
        {
            attemptingCarry = false;
            if(UpdateCarryTimerEvent != null)
                UpdateCarryTimerEvent(0);

            if(carryTimer <  _minTimeToCarry && 
                isCarrying == false)
                _currentSelection?.Use(this);
            else if(isCarrying)
                StopCarry();
            else
                StartCarry();
        }

        if(_inputState.MouseButton1.State == VButtonState.PRESS_START)
        {
            if(isCarrying) 
            {
                Rigidbody co = carriedObject;
                StopCarry();
                co.AddForce(_camera.forward * throwForce, ForceMode.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateCarriedObject();
    }
    
    private void CheckObject()
    {
        //Debug.DrawRay(camera.position, camera.forward * interactionRange);
        RaycastHit hit;
        if(Physics.Raycast(_camera.position, _camera.forward, out hit, _interactionRange, _interactionMask))
        {
            // Important to use collider.transform and not hit.transform (Gets parent collider transform)
            Usable usable = hit.collider.transform.GetComponent<Usable>();
            if(usable) 
            {
                ChangeSelection(usable);
                return;
            }
        }

        ClearSelection();
    }

    private void ClearSelection()
    {
        _currentSelection?.Highlight(false);
        _currentSelection = null;

        if(SelectionChangeEvent != null)
            SelectionChangeEvent("");
    }

    private void ChangeSelection(Usable usable)
    {
        if(_currentSelection) ClearSelection();
        usable.Highlight(true);
        _currentSelection = usable;

        if(SelectionChangeEvent != null)
            SelectionChangeEvent(_currentSelection.GetText(this));
    }

    private void StartCarry()
    {
        if(_currentSelection == null) return;
        
        PhysicalObject po = _currentSelection.GetComponent<PhysicalObject>();
        if(po == null || _characterStats.CanPickup(po.GetWeight()) == false)
            return;

        isCarrying = true;
        carriedObject = _currentSelection.transform.GetComponent<Rigidbody>();
        
        switch(carryType)
        {
            case CarryType.FIXED:

                carriedObject.isKinematic = true;
                carriedObject.freezeRotation = true;
                carriedObject.transform.SetParent(_carryPoint);

                break;
            case CarryType.PHYSICS:

                carriedObject.useGravity = false;

                oldDrag = carriedObject.drag;
                oldAngularDrag = carriedObject.angularDrag;

                carriedObject.drag = drag;
                carriedObject.angularDrag = angularDrag;

                carriedObject.transform.parent = _carryPoint;
                
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

    private void StopCarry()
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

    private void UpdateCarriedObject()
    {
        if(isCarrying == false || carryType != CarryType.PHYSICS)
            return;

        if(Vector3.Distance(carriedObject.position, _carryPoint.position) > .05f)
        {
            Vector3 moveDir = _carryPoint.position - carriedObject.position;
            carriedObject.AddForce(moveDir * moveForce);
        }
    }

    public void ChargeChakra(ChakraType type, float amount)
    {
        _characterStats.ChargeChakra(type, amount);
    }

    public bool AddItem(Item item)
    {
        return _inventory.TryAdd(item.CreateInstance());
    }

    public int GetMoney()
    {
        return _characterStats.Money;
    }

    public void AddMoney(int amount)
    {
        _characterStats.AddMoney(amount);
    }

    public bool DepleteMoney(int amount, bool greedy = false)
    {
        return _characterStats.DepleteMoney(amount, greedy);
    }

    public void CommitSteal()
    {
        _characterStats.CommitSteal();
    }

    private void OnDrawGizmos()
    {
        if(!debugView) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_camera.position, _camera.forward * _interactionRange);

        if(!joint) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(joint.transform.position, .05f);
    }
}