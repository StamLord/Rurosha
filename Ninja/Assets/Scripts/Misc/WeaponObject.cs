﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    public Animator animator;

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
