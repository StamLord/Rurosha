using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : WeaponObject, IHitboxResponder
{
    [Header("Hitbox")]
    [SerializeField] private Hitbox hitbox;
    
    [Header("Valid States for attack")]
    [SerializeField] private string[] validLeftAttackStates;
    [SerializeField] private string[] validRightAttackStates;
    [SerializeField] private string[] validSheathStates;

    [Header("Guard")]
    [SerializeField] private Collider guard;
    [SerializeField] private ParticleSystem guardVfx;
    [SerializeField] private float perfectGuardTime = .2f;
    [SerializeField] private float perfectGuardFollowupTime = 1f;
    
    private bool afterPerfectGuard; // True after perfect guard
    private float lastPerfectGuard; // Time of last perfect guard
    private Rigidbody perfectGuardTarget; // Target of the perfect guard follow up attack
    private bool perfectGuardFollowIsPlaying;

    //[Header("Movement")]
    private float axisInputWindow = .2f;

    private float lastX;
    private float lastZ;

    private bool positiveAxisPressX;
    private bool negativeAxisPressX;

    private bool positiveAxisPressZ;
    private bool negativeAxisPressZ;

    private float axisStartX;
    private float axisStartZ;

    [Header("Slice")]
    [SerializeField] private float sliceForce = 2f;
    [SerializeField] private float maxSlicesPerCut = 10;
    [SerializeField] private List<GameObject> newSlices = new List<GameObject>();
    
    [Header("Light Attack")]
    [SerializeField] private Damage lightAttackDamage;
    [SerializeField] private string[] lightAttackStates;

    [Header("Heavy Attack")]
    [SerializeField] private Damage heavyAttackDamage;
    [SerializeField] private string[] heavyAttackStates;

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
    [SerializeField] private float strengthExpGain = .01f;
    [SerializeField] private float dexterityExpGain = .01f;
    
    [Header("Stun Settings")]
    [SerializeField] private float staminaCostOnGuardedAttack;
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private bool stunned;

    private bool inAirState;

    public void SetAirState(bool state)
    {
        inAirState = state;
        if(state == false)
            animator.SetBool("Air Stab", false);
    }

    private void Start()
    {
        hitbox?.SetResponder(this);
        hitbox.SetIgnoreTransform(transform.root);
        pool = new Pool(projectile, 1);
    }

    private void Update()
    {
        ProcessInput();
        AutoSheath();
        ChargeVfx();
    }

    // Called by Hitbox on collision
    public void CollisionWith(Collider collider, Hitbox hitbox)
    {   
        //Gain Exp
        charStats.IncreaseAttributeExp(AttributeType.STRENGTH, strengthExpGain);
        
        //Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox)
        {
            // Heavy Attack
            if(ValidateState(heavyAttackStates))
                hurtbox.Hit(agent, heavyAttackDamage.softDamage, heavyAttackDamage.hardDamage, DamageType.Slash);
            // Light Attack
            else
                hurtbox.Hit(agent, lightAttackDamage.softDamage, lightAttackDamage.hardDamage, DamageType.Slash);
        }

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
        
        // Inform the guarding object of the Perfect Guard
        GuardDelegate guard = collider.GetComponent<GuardDelegate>();
        if(guard)
            guard.PerfectGuard(manager.Rigidbody);

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

    // Called by GuardDelegate when we performed perfect guard
    public override void PerfectGuard(Rigidbody target)
    {
        afterPerfectGuard = true;
        lastPerfectGuard = Time.time;
        perfectGuardTarget = target;
    }

    private void MovementCheck()
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

    protected override void DrawAnimation()
    {
        animator.Play("katana_draw");
    }

    protected override void SheathAnimation()
    {
        animator.Play("katana_sheath");
    }

    public void ProcessInput()
    {
        if(stunned)
            return;
        
        // Draw on input
        if(drawn == false)
        {
            if(inputState.MouseButton1.State == VButtonState.PRESS_START 
            || inputState.MouseButton2.State == VButtonState.PRESS_START
            || inputState.Defend.State == VButtonState.PRESS_START)
            {
                DrawWeapon();
                return;
            }
        }

        // Draw / Sheath button
        if(inputState.Draw.State == VButtonState.PRESS_START)
        {
            DrawSheathWeapon();
            return;
        }

        // Defend
        bool defend = inputState.Defend.Pressed;
        animator.SetBool("Defending", defend);
        guard.enabled = defend;

        // Perfect vs Regular guard
        if(guard.enabled)
            guard.gameObject.layer = (inputState.Defend.PressTime > perfectGuardTime)? LayerMask.NameToLayer("Guard") : LayerMask.NameToLayer("PerfectGuard");

        if(defend)
        {
            ResetAutoSheathTimer();
            return;
        }
        
        // LMB
        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
        {
            LeftAttack();
        }
        // RMB Pressed
        else if(inputState.MouseButton2.State == VButtonState.PRESS_START)
        {
            if(inAirState)
                animator.SetBool("Air Stab", true);
            else
                animator.SetBool("CHARGE RMB", true);

            ResetAutoSheathTimer();
        }
        else if(inputState.MouseButton2.Pressed)
        {
            if(inAirState == false)
                chargeTime = inputState.MouseButton2.PressTime;

            ResetAutoSheathTimer();
        }
        // RMB End Press
        else if(inputState.MouseButton2.State == VButtonState.PRESS_END)
        {
            if(animator.GetBool("Air Stab"))
                animator.SetBool("Air Stab", false);
            
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

            ResetAutoSheathTimer();
        }
    }

    private void ChargeVfx()
    {
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

    protected override void DrawSheathWeapon()
    {
        if (drawn == false)
            DrawWeapon();
        else if(ValidateState(validSheathStates))
            SheathWeapon();
    }

    private void LeftAttack()
    {
        if(perfectGuardFollowIsPlaying == false 
            && afterPerfectGuard 
            && Time.time - lastPerfectGuard <= perfectGuardFollowupTime)
        {
            StartCoroutine(PerfectGuardFollowUpAttackHorizontal(perfectGuardTarget));
        }
        else if(inputState.Crouch.State == VButtonState.PRESSED)
        {
            animator.SetTrigger("CrouchAttack");
            charStats.IncreaseAttributeExp(AttributeType.DEXTERITY, dexterityExpGain);
        }
        else if(ValidateState(validLeftAttackStates))
        {
            if(perfectGuardFollowIsPlaying == false)
            {
                animator.SetTrigger("LMB");
                charStats.IncreaseAttributeExp(AttributeType.DEXTERITY, dexterityExpGain);
            }
        }

        ResetAutoSheathTimer();
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

    IEnumerator PerfectGuardFollowUpAttackHorizontal(Rigidbody target)
    {
        perfectGuardFollowIsPlaying = true;

        Vector3 start = manager.Rigidbody.position;
        // We end up to the right and back of the target
        Vector3 end = target.position + target.transform.right * 1f - target.transform.forward * .5f;

        float startTime = Time.time;
        float duration = .2f;

        animator.Play("katana_perfect_followup_horizontal");
        manager.Rigidbody.isKinematic = true;
        while(Time.time - startTime <= duration)
        {
            float p = (Time.time - startTime) / duration;
            manager.Rigidbody.MovePosition(Vector3.Lerp(start, end, p));
            yield return null;
        }
        manager.Rigidbody.isKinematic = false;

        afterPerfectGuard = false;
        perfectGuardFollowIsPlaying = false;
    }

    IEnumerator PerfectGuardFollowUpAttack(Rigidbody target)
    {
        perfectGuardFollowIsPlaying = true;

        Vector3 start = manager.Rigidbody.position;
        Vector3 direction = (target.position - start).normalized;
        Vector3 end = target.position - direction * .5f;

        float startTime = Time.time;
        float duration = .2f;

        animator.Play("katana_perfect_followup_anticipation");
        manager.Rigidbody.isKinematic = true;
        while(Time.time - startTime <= duration)
        {
            float p = (Time.time - startTime) / duration;
            manager.Rigidbody.MovePosition(Vector3.Lerp(start, end, p));
            yield return null;
        }
        manager.Rigidbody.isKinematic = false;
        animator.Play("katana_perfect_followup");

        afterPerfectGuard = false;
        perfectGuardFollowIsPlaying = false;
    }
}
