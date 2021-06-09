using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : WeaponObject, IHitboxResponder
{
    [SerializeField] private CharacterStats charStats;
    [SerializeField] private Hitbox hitbox;

    [SerializeField] private bool nextAttack;
    [SerializeField] private int lastAttack; // 0 left 1 right

    [SerializeField] private int weaponSystem = 0; // 0 left 1 right
    
    [SerializeField] private float axisInputWindow = .2f;

    [SerializeField] private float lastX;
    [SerializeField] private float lastZ;

    [SerializeField] private bool positiveAxisPressX;
    [SerializeField] private bool negativeAxisPressX;

    [SerializeField] private bool positiveAxisPressZ;
    [SerializeField] private bool negativeAxisPressZ;

    [SerializeField] private float axisStartX;
    [SerializeField] private float axisStartZ;

    // Slice
    [SerializeField] private float sliceForce = 2f;
    [SerializeField] private float maxSlicesPerCut = 10;
    [SerializeField] private List<GameObject> newSlices = new List<GameObject>();

    [Header("Stats")]
    [SerializeField] private float leftAttackStaminaCost= 2f;
    [SerializeField] private float rightAttackStaminaCost= 2f;
    [SerializeField] private float stabAttackStaminaCost= 2f;
    [SerializeField] private float crouchAttackStaminaCost= 2f;

    [SerializeField] private float strengthExpGain = .01f;
    [SerializeField] private float dexterityExpGain = .01f;

    public enum Stance {Low, Medium, High}
    [SerializeField] private Stance stance = Stance.Medium;

    private Vector3 mouseDelta;
    private Vector3 lastMousePos;

    public delegate void stanceSwitchDeltaDelegate(Vector3 mouseDelta);
    public event stanceSwitchDeltaDelegate StanceSwitchDeltaEvent;

    public delegate void stanceSwitchStart();
    public event stanceSwitchStart StanceSwitchStartEvent;

    public delegate void stanceSwitchEnd();
    public event stanceSwitchEnd StanceSwitchEndEvent;

    void Start()
    {
        hitbox?.SetResponder(this);
    }

    void Update()
    {
        MovementCheck();

        if(Input.GetButtonDown("Switch"))
        {
            mouseDelta = Vector3.zero;
            lastMousePos = Input.mousePosition;
            if(StanceSwitchStartEvent != null)
                StanceSwitchStartEvent();
        }

        if(Input.GetButton("Switch"))
        {
            mouseDelta += (Input.mousePosition - lastMousePos);
            if(StanceSwitchDeltaEvent != null)
                StanceSwitchDeltaEvent(mouseDelta);
            
            lastMousePos = Input.mousePosition;
            Debug.Log(mouseDelta);
        }

        if(Input.GetButtonUp("Switch"))
        {
            if(mouseDelta.y > 0.2f)
                StanceChange(Stance.High);
            else if(mouseDelta.y < -0.2f)
                StanceChange(Stance.Low);
            else
                StanceChange(Stance.Medium);

            if(StanceSwitchEndEvent != null)
                StanceSwitchEndEvent();
        }

        switch(weaponSystem)
        {
            case 0:
                Method1();
                break;
            case 1:
                Method2();
                break;
        }

        //Debug.DrawRay(hitbox.transform.position, hitbox.transform.right);
    }

    private void StanceChange(Stance newStance)
    {
        if(stance == newStance)
            return;

        stance = newStance;
        switch(stance)
        {
            case Stance.Low:
                break;
            case Stance.Medium:
                animator.Play("Idle");
                break;
            case Stance.High:
                animator.Play("HighIdle");
                break;
        }
    }

    public void CollisionWith(Collider collider)
    {   
        //Gain Exp
        charStats.IncreaseAttributeExp("strength", strengthExpGain);
        
        //Collision Effects
        PhysicalMaterial pMat = collider.GetComponent<PhysicalMaterial>();
        pMat?.CollideEffect(collider.ClosestPointOnBounds(transform.position));
        
        //Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        hurtbox?.GetHit(10);

        //Slice
        Sliceable sliceable = collider.GetComponent<Sliceable>();
        if(sliceable)
        {
            // Check if max sliced objects were created
            if(newSlices.Count >= maxSlicesPerCut)
                return;

            // Check if this is a sliced object that was created by this same attack
            foreach(GameObject s in newSlices)
                if(s == collider.gameObject)
                    return;

            GameObject toSlice = collider.gameObject;

            Vector3 normal = hitbox.transform.right;
            Vector3 transformedNormal = ((Vector3)(toSlice.transform.localToWorldMatrix.transpose * normal)).normalized;
            Plane plane = new Plane(transformedNormal, toSlice.transform.InverseTransformPoint(hitbox.transform.position));

            StartCoroutine(SliceCoroutine(plane, toSlice));
        }
    }

    private IEnumerator SliceCoroutine(Plane plane, GameObject toSlice)
    {
        GameObject[] slices = Assets.Scripts.Slicer.Slice(plane, toSlice);

        // Add new sliced objects to a list of objects ignored in any further collisions
        foreach(GameObject s in slices)
            newSlices.Add(s);

        Destroy(toSlice);
        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        rigidbody.AddForce(hitbox.transform.up * sliceForce, ForceMode.Impulse);

        yield return null;
    }

    public void UpdateColliderState(bool newState)
    {
        if(newState == false)
            newSlices.Clear();
    }

    public void SetNextAttack(bool state)
    {
        nextAttack = state;
    }

    public void SetNextAttackTrue()
    {
        SetNextAttack(true);
    }

    public void SetNextAttackFalse()
    {
        SetNextAttack(false);
    }

    void MovementCheck()
    {
        // Horizontal Axis
        float currentX = Input.GetAxis("Horizontal");

        if(positiveAxisPressX == false && currentX > lastX && currentX > 0)
        {
            axisStartX = Time.time;
            positiveAxisPressX = true;
            negativeAxisPressX = false;
        }
        else if (positiveAxisPressX && currentX < lastX)
            positiveAxisPressX = false;

        if(negativeAxisPressX == false && currentX < lastX && currentX < 0)
        {
            axisStartX = Time.time;
            positiveAxisPressX = false;
            negativeAxisPressX = true;
        }
        else if (negativeAxisPressX && currentX > lastX)
            negativeAxisPressX = false;
        

        // Vertical Axis
        float currentZ = Input.GetAxis("Vertical");

        if(positiveAxisPressZ == false && currentZ > lastZ && currentZ > 0)
        {
            axisStartZ = Time.time;
            positiveAxisPressZ = true;
            negativeAxisPressZ = false;
        }
        else if (positiveAxisPressZ && currentZ < lastZ)
            positiveAxisPressZ = false;

        if(negativeAxisPressZ == false && currentZ < lastZ && currentZ < 0)
        {
            axisStartZ = Time.time;
            positiveAxisPressZ = false;
            negativeAxisPressZ = true;
        }
        else if (negativeAxisPressZ && currentZ > lastZ)
            negativeAxisPressZ = false;
        
        lastX = currentX;
        lastZ = currentZ;
    }

    public void Method1()
    {
        if(Input.GetMouseButtonDown(0))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            float currentTime = state.normalizedTime % 1;

            if(Input.GetButton("Crouch"))
            {
                if(charStats.DepleteStamina(crouchAttackStaminaCost))
                {
                    animator.SetTrigger("CrouchAttack");
                }
            }
            else if(state.IsName("Idle") || state.IsName("HighIdle") ||state.IsName("Attack1") && nextAttack)
            {
                if(charStats.DepleteStamina(leftAttackStaminaCost))
                {
                    animator.SetTrigger("Attack");
                    charStats.IncreaseAttributeExp("dexterity", dexterityExpGain);
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
            animator.SetBool("Defending", true);
        else if(Input.GetMouseButtonUp(1))
            animator.SetBool("Defending", false);
    }

    public void Method2()
    {
        // Defend
        if(Input.GetKeyDown(KeyCode.F) && nextAttack)
        {   
            animator.SetBool("Defending", true);
            return;
        }
        
        if(Input.GetKey(KeyCode.F) == false)
            animator.SetBool("Defending", false);
        
        // LMB
        if(Input.GetMouseButtonDown(0))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if(positiveAxisPressZ && Time.time - axisStartZ < axisInputWindow && state.IsName("Idle") ||
                positiveAxisPressZ && Time.time - axisStartZ < axisInputWindow && lastAttack == 0 && nextAttack)
            {
                if(charStats.DepleteStamina(stabAttackStaminaCost))
                    animator.SetTrigger("StabAttack");
            }
            else if(state.IsName("Idle") || state.IsName("HighIdle") || lastAttack == 1 && nextAttack)
            {    
                if(charStats.DepleteStamina(leftAttackStaminaCost))
                {
                    animator.SetTrigger("LAttack");
                    lastAttack = 0;
                }
            }
        }
        // RMB
        else if(Input.GetMouseButtonDown(1))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            
            if(positiveAxisPressZ && Time.time - axisStartZ < axisInputWindow && state.IsName("Idle") ||
                positiveAxisPressZ && Time.time - axisStartZ < axisInputWindow && lastAttack == 1 && nextAttack)
            {
                if(charStats.DepleteStamina(stabAttackStaminaCost))
                    animator.SetTrigger("StabAttack");
            }
            else if(state.IsName("Idle") || lastAttack == 0 && nextAttack)
            {   
                if(charStats.DepleteStamina(rightAttackStaminaCost))
                {
                    if(Input.GetButton("Crouch"))
                        animator.SetTrigger("CrouchAttack");
                    else
                    {
                        animator.SetTrigger("RAttack");
                        lastAttack = 1;
                    }
                }
            }
        }
    }
}
