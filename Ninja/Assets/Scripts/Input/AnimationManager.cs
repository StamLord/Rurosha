using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private InputState inputState;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private float runSpeed = 10;

    private void OnValidate() 
    {
        animator = GetComponent<Animator>();
    }
    
    private void LateUpdate()
    {
        Vector3 velocity = rigidbody.velocity.magnitude * inputState.AxisInput;
        animator.SetFloat("x", velocity.x);
        animator.SetFloat("z", velocity.z / runSpeed);
        animator.SetBool("crouch", inputState.Crouch.State == VButtonState.PRESSED);
    }
}
