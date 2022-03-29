using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : WeaponObject, IHitboxResponder
{
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

    [Header("Damage")]
    [SerializeField] private int softDamage = 20;
    [SerializeField] private int hardDamage = 10;
    [SerializeField] private float chanceToBleed = .25f;

    [Header("Stats")]
    [SerializeField] private float leftAttackStaminaCost= 2f;
    [SerializeField] private float rightAttackStaminaCost= 2f;
    [SerializeField] private float stabAttackStaminaCost= 2f;
    [SerializeField] private float crouchAttackStaminaCost= 2f;

    [SerializeField] private float strengthExpGain = .01f;
    [SerializeField] private float dexterityExpGain = .01f;
    
    [Header("Stance Settings")]
    [SerializeField] private float mouseDelta;
    [SerializeField] private float mouseDeltaMargin = 1f;
    [SerializeField] private KatanaStance stance = KatanaStance.Medium;

    public delegate void stanceSwitchDeltaDelegate(KatanaStance stance);
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

        StanceInput();

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

    private void StanceInput()
    {
        if(Input.GetButtonDown("Switch"))
        {
            mouseDelta = 0;
            if(StanceSwitchStartEvent != null)
                StanceSwitchStartEvent();
        }

        if(Input.GetButton("Switch"))
        {
            mouseDelta += Input.GetAxis("Mouse Y");
            KatanaStance tempStance;

            if(mouseDelta > mouseDeltaMargin)
                tempStance = KatanaStance.High;
            else if(mouseDelta < -mouseDeltaMargin)
                tempStance = KatanaStance.Low;
            else
                tempStance = KatanaStance.Medium;

            if(StanceSwitchStartEvent != null)
                StanceSwitchDeltaEvent(tempStance);
        }

        if(Input.GetButtonUp("Switch"))
        {
            if(mouseDelta > mouseDeltaMargin)
                StanceChange(KatanaStance.High);
            else if(mouseDelta < -mouseDeltaMargin)
                StanceChange(KatanaStance.Low);
            else
                StanceChange(KatanaStance.Medium);

            if(StanceSwitchEndEvent != null)
                StanceSwitchEndEvent();
        }
    }

    private void StanceChange(KatanaStance newStance)
    {
        if(stance == newStance)
            return;

        stance = newStance;
        switch(stance)
        {
            case KatanaStance.Low:
                break;
            case KatanaStance.Medium:
                animator.Play("Idle");
                break;
            case KatanaStance.High:
                animator.Play("HighIdle");
                break;
        }
    }

    // Called by Hitbox on collision
    public void CollisionWith(Collider collider)
    {   
        //Gain Exp
        charStats.IncreaseAttributeExp("strength", strengthExpGain);
        
        //Collision Effects
        PhysicalMaterial pMat = collider.GetComponent<PhysicalMaterial>();
        pMat?.CollideEffect(collider.ClosestPoint(transform.position), hardDamage);
        
        //Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        hurtbox?.Hit(softDamage, hardDamage, DamageType.Slash);

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
        float currentX = inputState.AxisInput.x;

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
        float currentZ = inputState.AxisInput.z;

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
        // MB1  + MB2 = Defend
        bool defend = (inputState.MouseButton1.Pressed && inputState.MouseButton2.Pressed);
        animator.SetBool("Defending", defend);

        if(defend)
            return;
        
        // LMB
        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            float currentTime = state.normalizedTime % 1;

            if(inputState.Crouch.State == VButtonState.PRESSED)
            {
                if(charStats.DepleteStamina(crouchAttackStaminaCost))
                {
                    animator.SetTrigger("CrouchAttack");
                    charStats.IncreaseAttributeExp("dexterity", dexterityExpGain);
                }
            }
            else if(state.IsName("Idle") || state.IsName("HighIdle") || state.IsName("Attack1_chain") || state.IsName("Attack2_chain"))
            {
                if(charStats.DepleteStamina(leftAttackStaminaCost))
                {
                    animator.SetTrigger("LMB");
                    charStats.IncreaseAttributeExp("dexterity", dexterityExpGain);
                }
            }
        }
        // RMB
        else if(inputState.MouseButton2.State == VButtonState.PRESS_START)
        {
            animator.SetTrigger("RMB");
            charStats.IncreaseAttributeExp("dexterity", dexterityExpGain);
        }
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
        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
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
        else if(inputState.MouseButton2.State == VButtonState.PRESS_START)
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
                    if(inputState.Crouch.State == VButtonState.PRESSED)
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
