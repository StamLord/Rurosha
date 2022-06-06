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

    [Header("Combo")]
    [SerializeField] private List<string> currentCombo;
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private float comboLastAttackTime;
    [SerializeField] private int maxComboLength = 5;
    [SerializeField] private List<Combo> combos = new List<Combo>();
    
    [System.Serializable] public struct Combo
    {
        public float softDamage;
        public float hardDamage;
        public string combo;
        public string animationState;
    }

    private void Start() 
    {
        foreach(Hitbox h in hitboxes)
            h.SetResponder(this);
    }

    void Update()
    {
        Input();

        // Reset combo when enough time since last attack passes
        if(Time.time - comboLastAttackTime >= comboResetTime)    
            ResetCombo();
    }

    private void Input()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // Defend (LMB + RMB)
        bool defend = (inputState.MouseButton1.State == VButtonState.PRESSED &&
        inputState.MouseButton2.State == VButtonState.PRESSED);
        animator.SetBool("DEFEND", defend);
        if(defend)
            return;
        
        // Left Attack (LMB)
        if(inputState.MouseButton1.State == VButtonState.PRESS_END)
        {
            if(state.IsName("idle") || state.IsName("fist_left_chain") || state.IsName("fist_right_chain"))
            {
                if(charStats.DepleteStamina(leftAttackStaminaCost))
                {
                    AddCombo("left");
                    if(VerifyCombo() == false)
                        animator.SetTrigger("LMB");
                    charStats.IncreaseAttributeExp("dexterity", dexterityExpGain);
                }
            }
        }
        // Right Attack (RMB)
        else if(inputState.MouseButton2.State == VButtonState.PRESS_END)
        {
            if(state.IsName("idle") || state.IsName("fist_left_chain") || state.IsName("fist_right_chain"))
            {
                if(charStats.DepleteStamina(rightAttackStaminaCost))
                {
                    AddCombo("right");
                    if(VerifyCombo() == false)
                        animator.SetTrigger("RMB");
                    charStats.IncreaseAttributeExp("dexterity", dexterityExpGain);
                }
            }
        }
    }

    private void AddCombo(string attack)
    {
        currentCombo.Add(attack);
        comboLastAttackTime = Time.time;
    }

    private void ResetCombo()
    {
        currentCombo.Clear();
    }

    private bool VerifyCombo()
    {
        // Turn list to string for easy comparison.
        // Example: ["left", "left", "right"] => "left,left,right"
        //string current = string.Join(",", currentCombo.ToArray());
        
        // Search for a combo that matches our current set of attacks
        foreach(Combo c in combos)
        {
            if(ComboMatches(currentCombo.ToArray(), c.combo))
            {
                // Transition to animation
                animator.CrossFade(c.animationState, 0.05f);
                ResetCombo();
                return true;
            }
        }
        return false;
    }

    private bool ComboMatches(string[] attacks, string combo)
    {
        // Turn string of combo to array
        string[] comboArr = combo.Split(',');
        
        // If combo is larger than number of attacks, we don't match it
        if(comboArr.Length > attacks.Length) return false;

        // Loop over combo
        // If all attacks performed match combo (we check from last to first), return true
        for (var i = 0; i < comboArr.Length; i++)
        {
            if(comboArr[comboArr.Length - 1 - i] != attacks[attacks.Length - 1 - i])
                return false;
        }

        return true;
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        // Don't collider with self
        if(collider.transform.root == transform.root) return;

        //Gain Exp
        charStats.IncreaseAttributeExp("strength", strengthExpGain);
        
        //Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox) hurtbox.Hit(softDamage, hardDamage, DamageType.Blunt);

        //Physics
        Rigidbody rb = collider.GetComponent<Rigidbody>();
        if(rb) rb.AddForce(hitbox.Velocity * fistForce, ForceMode.Impulse);
    }

    public void GuardedBy(Collider collider, Hitbox hitbox)
    {
        // Play guarded animation
        // Depelte stamina
        // Stun if run out of stamina
    }
    
    public void UpdateColliderState(bool state)
    {
         
    }
}
