using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiRingUI : UIWindow
{
    [Header("References")]
    [SerializeField] private Animator animator;

    public void Show()
    {
        animator.Play("show");
    }

    public void Hide()
    {
        animator.Play("hide");
    }
}
