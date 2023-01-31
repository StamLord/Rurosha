using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected WeaponManager manager;
    [SerializeField] protected Animator animator;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    protected InputState inputState {get {return manager.InputState;}}
    protected CharacterStats charStats {get {return manager.Stats;}}
    protected StealthAgent agent {get {return manager.Agent;}}
    protected new Camera camera {get {return manager.Camera;}}

    [Header("Debug Info")]
    [SerializeField] protected bool drawn;
    [SerializeField] protected Item item;

    [System.Serializable]
    public struct Damage
    {
        public int softDamage;
        public int hardDamage;
    }

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        //InitializeOutline();
        if(animator == null) animator = GetComponent<Animator>();
    }

    public void SetWeaponManager(WeaponManager manager)
    {
        this.manager = manager;
    }

    public virtual void SetItem(Item item)
    {
        this.item = item;
        UpdateVisual();
    }

    protected virtual void DrawAnimation()
    {
        animator.Play("Draw");
    }

    protected virtual void SheathAnimation()
    {
        animator.Play("Sheeth");
    }

    protected void UseAnimation()
    {
        animator.Play("Use");
    }

    protected void AltUseAnimation()
    {
        animator.Play("AltUse");
    }
    
    public virtual void PerfectGuard(Rigidbody target)
    {

    }

    public virtual void Guard(Rigidbody target)
    {
        
    }

    protected virtual void DrawWeapon()
    {
        drawn = true;
        DrawAnimation();
    }

    protected virtual void SheathWeapon()
    {
        drawn = false;
        SheathAnimation();
    }

    protected void UpdateVisual()
    {
        if(meshFilter) meshFilter.mesh = item.model;
        if(meshRenderer) meshRenderer.material = item.material;
    }

    protected bool ValidateState(string[] states)
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        for (int i = 0; i < states.Length; i++)
        {
            if(state.IsName(states[i]))
                return true;
        }

        return false;
    }
}
