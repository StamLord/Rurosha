using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : WeaponObject, IHitboxResponder
{
    [Header("Hitboxes")]
    [SerializeField] private Hitbox[] hitboxes;

    [Header("Attack Settings")]
    [SerializeField] private int softDamage;
    [SerializeField] private int hardDamage;

    [SerializeField] private float leftAttackStaminaCost;
    [SerializeField] private float rightAttackStaminaCost;
    
    [Header("Experience Settings")]
    [SerializeField] private float strengthExpGain;
    [SerializeField] private float dexterityExpGain;

    [Header("Physics Settings")]
    [SerializeField] private float fistForce;

    private void Start() 
    {
        foreach(Hitbox h in hitboxes)
            h.SetResponder(this);
    }

    void Update()
    {
        Input();    
    }

    private void Input()
    {
        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if(state.IsName("idle") || state.IsName("fist_left_chain") || state.IsName("fist_right_chain"))
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
            if(charStats.DepleteStamina(rightAttackStaminaCost))
            {
                animator.SetTrigger("RMB");
                charStats.IncreaseAttributeExp("dexterity", dexterityExpGain);
            }
        }
    }

    public void CollisionWith(Collider collider)
    {
        // Don't collider with self
        if(collider.transform.root == transform.root) return;
        
        //Gain Exp
        charStats.IncreaseAttributeExp("strength", strengthExpGain);
        
        //Collision Effects
        PhysicalMaterial pMat = collider.GetComponent<PhysicalMaterial>();
        if(pMat) pMat.CollideEffect(collider.ClosestPointOnBounds(transform.position));
        
        //Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox) hurtbox.Hit(softDamage, hardDamage, DamageType.Blunt);

        //Physics
        Rigidbody rb = collider.GetComponent<Rigidbody>();
        if(rb) rb.AddForce(transform.forward * fistForce, ForceMode.Impulse);
    }

    public void UpdateColliderState(bool state)
    {
         
    }
}
