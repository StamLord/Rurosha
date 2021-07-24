using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected InputState inputState;
    [SerializeField] protected CharacterStats charStats;

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        //InitializeOutline();
        animator = GetComponent<Animator>();
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
