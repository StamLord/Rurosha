using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected WeaponManager manager;
    [SerializeField] protected Animator animator;
    protected InputState inputState {get {return manager.InputState;}}
    protected CharacterStats charStats {get {return manager.Stats;}}
    protected StealthAgent agent {get {return manager.Agent;}}
    protected new Camera camera {get {return manager.Camera;}}

    public bool drawn;

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
    
    // private void OnEnable() 
    // {
    //     DrawWeapon();
    // }

    // private void OnDisable() 
    // {
    //     SheathWeapon();    
    // }

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
}
