using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bo : WeaponObject, IHitboxResponder
{
    [Header("Damage")]
    [SerializeField] private int softDamage = 20;
    [SerializeField] private int hardDamage = 10;
    [SerializeField] private float chanceToBleed = .25f;

    [Header("Guard")]
    [SerializeField] private Collider guard;

    [Header("Valid States to attack")]
    [SerializeField] private string[] validLeftAttackStates;
    [SerializeField] private string[] validRightAttackStates;

    [Header("Experience")]
    [SerializeField] private float strengthExpGain = .01f;
    [SerializeField] private float dexterityExpGain = .01f;

    [Header("Hitbox")]
    [SerializeField] private Hitbox[] hitboxes;

    [Header("Combo")]
    [SerializeField] private int maxCombo = 3;
    [SerializeField]public List<Direction9> combo = new List<Direction9>();
    [SerializeField] private bool specialAttackReady;
    [SerializeField] private Direction9 lastAttackDirection;
    [SerializeField] private Direction9 lastComboAttackDirection;

    [Header("Physics")]
    [SerializeField] private float pushForce = 20f;

    private void Start()
    {
        foreach(Hitbox h in hitboxes)
        {
            h.SetResponder(this);
            h.SetIgnoreTransform(transform.root);
        }
    }

    private void Update()
    {
        ProcessAttackInput();                
    }

    private void ProcessAttackInput()
    {
        // Guard
        bool guarding = inputState.Defend.State == VButtonState.PRESSED;
        Guard(guarding);
        
        // Left mouse button
        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
        {
            LeftAttack();
        }
        // Right mouse button
        else if(inputState.MouseButton2.State == VButtonState.PRESS_START)
        {
            RightAttack();
        }
    }

    private void Guard(bool isGuarding)
    {
        animator.SetBool("GUARD", isGuarding);
        guard.enabled = isGuarding;
    }

    private void LeftAttack()
    {
        if(ValidateState(validLeftAttackStates))
            animator.SetTrigger("LMB");
    }

    private void RightAttack()
    {
        if(ValidateState(validRightAttackStates))
            animator.SetTrigger("RMB");
    }

    // Called by Hitbox on collision
    public void CollisionWith(Collider collider, Hitbox hitbox)
    {   
        // Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        bool isHit = false;
        if(hurtbox)
            isHit = hurtbox.Hit(agent, softDamage, hardDamage, DamageType.Blunt, lastComboAttackDirection);
        
        if(isHit)
        {   
            // Gain Exp
            charStats.IncreaseAttributeExp(AttributeType.STRENGTH, strengthExpGain);
        }

        // Physics
        Rigidbody rb = collider.GetComponent<Rigidbody>();
        if(rb) rb.AddForce(hitbox.Velocity * pushForce, ForceMode.Impulse);
    }

    private void AddCombo(Direction9 hitDirection)
    {
        // Make sure it's not the last direction added to combo
        // Handles cases when combo is cleared but another collision registers by same attack immedietly adding same direction
        if(lastComboAttackDirection == hitDirection)
            return;

        // Make sure direction is unique in combo
        if(combo.Contains(hitDirection))
            return;
        
        combo.Add(hitDirection);
        lastComboAttackDirection = hitDirection;

        if(combo.Count >= maxCombo)
        {
            combo.Clear();
            specialAttackReady = true;
        }
    }

    public void UpdateColliderState(bool newState)
    {
        return;
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
}
