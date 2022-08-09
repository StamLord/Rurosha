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
    [SerializeField] private bool drawn;
    [SerializeField] private Item item;

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

    protected void UpdateVisual()
    {
        if(meshFilter) meshFilter.mesh = item.model;
        if(meshRenderer) meshRenderer.material = item.material;
    }
}
