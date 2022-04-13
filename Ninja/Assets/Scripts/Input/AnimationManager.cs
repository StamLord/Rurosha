using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private InputState inputState;

    void LateUpdate()
    {
        if(animator)
        {
            animator.SetFloat("Speed X", inputState.AxisInput.x);
            animator.SetFloat("Speed Z", inputState.AxisInput.z);
            animator.SetBool("Crouching", inputState.Crouch.State == VButtonState.PRESSED);
        }
    }
}
