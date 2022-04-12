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

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        //InitializeOutline();
        animator = GetComponent<Animator>();
    }

    public void SetWeaponManager(WeaponManager manager)
    {
        this.manager = manager;
    }

    public void DrawAnimation()
    {
        animator.Play("Draw");
    }

    public void SheethAnimation()
    {
        animator.Play("Sheeth");
    }

    public void UseAnimation()
    {
        animator.Play("Use");
    }

    public void AltUseAnimation()
    {
        animator.Play("AltUse");
    }

    
}
