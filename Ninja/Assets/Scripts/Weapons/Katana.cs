using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : WeaponObject, IHitboxResponder
{
    [Header("Hitbox")]
    [SerializeField] private Hitbox hitbox;

    [Header("Guard")]
    [SerializeField] private Collider guard;
    [SerializeField] private float perfectGuardTime = .2f;
    [SerializeField] private ParticleSystem guardVfx;

    [SerializeField] private bool nextAttack;
    [SerializeField] private int lastAttack; // 0 left 1 right

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

    [Header("Charge Settings")]
    [SerializeField] private float minCharge = .2f;
    [SerializeField] private float maxCharge = 2f;
    [SerializeField] private ParticleSystem chargingVfx;
    [SerializeField] private ParticleSystem chargedVfx;
    [SerializeField] private ParticleSystem chargeEndVfx;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float specialProjDistance;
    [SerializeField] private float specialProjSpeed;
    private float chargeTime;
    private bool chargeEndPlayed;
    private Pool pool;

    [Header("Stats")]
    [SerializeField] private float leftAttackStaminaCost= 2f;
    [SerializeField] private float rightAttackStaminaCost= 2f;
    [SerializeField] private float stabAttackStaminaCost= 2f;
    [SerializeField] private float crouchAttackStaminaCost= 2f;

    [SerializeField] private float strengthExpGain = .01f;
    [SerializeField] private float dexterityExpGain = .01f;
    
    [Header("Stun Settings")]
    [SerializeField] private float staminaCostOnGuardedAttack;
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private bool stunned;

    void Start()
    {
        hitbox?.SetResponder(this);
        hitbox.SetIgnoreTransform(transform.root);
        pool = new Pool(projectile, 1);
    }

    void Update()
    {
        MovementCheck();
        Method1();
    }

    // Called by Hitbox on collision
    public void CollisionWith(Collider collider, Hitbox hitbox)
    {   
        //Gain Exp
        charStats.IncreaseAttributeExp(AttributeType.STRENGTH, strengthExpGain);
        
        //Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox)
            hurtbox.Hit(agent, softDamage, hardDamage, DamageType.Slash);

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

    public void GuardedBy(Collider collider, Hitbox hitbox)
    {
        // VFX
        guardVfx?.Play();
    }

    public void PerfectGuardedBy(Collider collider, Hitbox hitbox)
    {
        // Depelte stamina
        bool outOfStamina = !charStats.DepleteStamina(staminaCostOnGuardedAttack, true);

        // Stun if run out of stamina
        if(outOfStamina) 
            Stun();
        else // Play guarded animation
        {
            animator.Play("katana_blocked"); // Player
            animator.SetTrigger("Blocked"); // NPC
        }

        // VFX
        guardVfx?.Play();
    }

    private void Stun()
    {
        StartCoroutine("StunCoroutine");
    }

    private IEnumerator StunCoroutine()
    {
        stunned = true;
        animator.Play("katana_stun_start");
        animator.SetBool("Stun", true);
        yield return new WaitForSeconds(stunDuration);
        stunned = false;
        animator.SetBool("Stun", false);
    }

    private IEnumerator SliceCoroutine(Plane plane, GameObject toSlice)
    {
        GameObject[] slices = Slicer.Slice(plane, toSlice);

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
        if(stunned)
            return;
        
        // Defend
        bool defend = inputState.Defend.Pressed;
        animator.SetBool("Defending", defend);
        guard.enabled = defend;

        // Perfect vs Regular guard
        if(guard.enabled)
            guard.gameObject.layer = (inputState.Defend.PressTime > perfectGuardTime)? LayerMask.NameToLayer("Guard") : LayerMask.NameToLayer("PerfectGuard");

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
                    charStats.IncreaseAttributeExp(AttributeType.DEXTERITY, dexterityExpGain);
                }
            }
            else if(state.IsName("Idle") || state.IsName("HighIdle") || state.IsName("Attack1_chain") || state.IsName("Attack2_chain"))
            {
                if(charStats.DepleteStamina(leftAttackStaminaCost))
                {
                    animator.SetTrigger("LMB");
                    charStats.IncreaseAttributeExp(AttributeType.DEXTERITY, dexterityExpGain);
                }
            }
        }
        // RMB Pressed
        else if(inputState.MouseButton2.Pressed)
        {
            animator.SetBool("CHARGE RMB", true);
            chargeTime = inputState.MouseButton2.PressTime;
        }
        // RMB End Press
        else if(inputState.MouseButton2.State == VButtonState.PRESS_END)
        {
            animator.SetBool("CHARGE RMB", false);

            // Create projectile slash
            if(chargeTime >= maxCharge)
            {
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("katana_right_1_charge"))
                    SpecialAttack(Direction9.UP);
                else if(animator.GetCurrentAnimatorStateInfo(0).IsName("katana_right_2_charge"))
                    SpecialAttack(Direction9.RIGHT);
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("katana_right_3_charge"))
                    SpecialAttack(Direction9.DOWNRIGHT);
            }
            chargeTime = 0;
            chargeEndPlayed = false;

            charStats.IncreaseAttributeExp(AttributeType.DEXTERITY, dexterityExpGain);
        }

        // VFX
        if(chargeTime > .2f)
        {
            // Fully charged
            if(chargeTime >= maxCharge)
            {
                if(chargeEndPlayed == false)
                {
                    chargeEndVfx.Play();
                    chargingVfx.Stop();
                    chargedVfx.Play();
                    chargeEndPlayed = true;
                }
            }
            // Charging
            else
            {
                if(chargingVfx.isPlaying == false)
                    chargingVfx.Play();
            }
        }
        // VFX End
        else
        {
            if(chargingVfx && chargingVfx.isPlaying) chargingVfx.Stop();
            if(chargedVfx && chargedVfx.isPlaying) chargedVfx.Stop();
        }
    }

    private void SpecialAttack(Direction9 direction)
    {
        float z = 0;

        if(direction == Direction9.LEFT || direction == Direction9.RIGHT)
            z = 90;
        else if(direction == Direction9.UPLEFT || direction == Direction9.DOWNRIGHT)
            z = 45;
        else if(direction == Direction9.UPRIGHT || direction == Direction9.DOWNLEFT)
            z = -45;
        
        GameObject p = pool.Get();
        p.transform.position = hitbox.transform.position + transform.forward;
        p.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y ,z);
        Hitbox hit = p.GetComponent<Hitbox>();
        if(hit) 
        {
            hit.SetResponder(this);
            StartCoroutine("SpecialProjectile", hit);
        }
    }

    IEnumerator SpecialProjectile(Hitbox projectile)
    {
        Vector3 origin = projectile.transform.position;
        while(Vector3.Distance(projectile.transform.position, origin) < specialProjDistance)
        {
            projectile.transform.position += projectile.transform.forward * specialProjSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        projectile.ForgetCollided(); // Forget all colided objects so far so we can reuse this object
        pool.Return(projectile.gameObject);
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
