using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kanabo : WeaponObject, IHitboxResponder
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

    [Header("Stats")]
    [SerializeField] private float leftAttackStaminaCost= 2f;
    [SerializeField] private float highAttackStaminaCost= 2f;
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
        charStats.IncreaseAttributeExp("strength", strengthExpGain);
        
        PhysicalMaterial pMat = collider.GetComponent<PhysicalMaterial>();
        pMat?.CollideEffect(collider.ClosestPointOnBounds(transform.position));
        
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        hurtbox?.Hit(30, 10);
    }

    public void UpdateColliderState(bool newState)
    {
        
    }

    public void SetNextAttack(bool state)
    {
        nextAttack = state;
    }

    public void SetNextAttackTrue()
    {
        nextAttack = true;
    }

    public void SetNextAttackFalse()
    {
        nextAttack = false;
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

            if(Input.GetButton("Crouch"))
            {
                if(charStats.DepleteStamina(crouchAttackStaminaCost))
                {
                    animator.SetTrigger("CrouchAttack");
                }
            }
            else if(state.IsName("Idle") ||state.IsName("Fire1") && nextAttack || state.IsName("Fire2") && nextAttack)
            {
                if(charStats.DepleteStamina(leftAttackStaminaCost))
                {
                    animator.SetTrigger("Attack");
                    charStats.IncreaseAttributeExp("strength", strengthExpGain);
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if(state.IsName("Idle")) 
            {
                if(charStats.DepleteStamina(highAttackStaminaCost))
                {
                    animator.SetTrigger("RAttack");
                    charStats.IncreaseAttributeExp("strength", strengthExpGain);
                }
            }
            else if(state.IsName("Fire1") && nextAttack)
            {
                if(charStats.DepleteStamina(stabAttackStaminaCost))
                {
                    animator.SetTrigger("RAttack");
                    charStats.IncreaseAttributeExp("strength", strengthExpGain);
                }
            }
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
                // if(charStats.DepleteStamina(rightAttackStaminaCost))
                // {
                //     if(Input.GetButton("Crouch"))
                //         animator.SetTrigger("CrouchAttack");
                //     else
                //     {
                //         animator.SetTrigger("RAttack");
                //         lastAttack = 1;
                //     }
                // }
            }
        }
    }
}
