﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    [SerializeField] private CharacterStateMachine stateMachine;
    [SerializeField] private Animator animator;
    [SerializeField] private InputState inputState;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private float runSpeed = 10;

    [SerializeField] private Transform ikLookTarget;
    [SerializeField] private Transform ikLeftTarget;
    [SerializeField] private Transform ikRightTarget;

    private void OnValidate() 
    {
        animator = GetComponent<Animator>();    
    }

    private void Start()
    {
        stateMachine.OnStateChange += StateUpdate;
    }
    
    private void LateUpdate()
    {
        Vector3 velocity = rigidbody.velocity.magnitude * inputState.AxisInput;
        animator.SetFloat("x", velocity.x);
        animator.SetFloat("z", velocity.z / runSpeed);
        animator.SetFloat("y", rigidbody.velocity.y);
        animator.SetBool("crouch", inputState.Crouch.State == VButtonState.PRESSED);
    }

    private void StateUpdate(string stateName)
    {
        switch(stateName)
        {
            case "GroundedState":
                animator.CrossFade("grounded", .1f);
                break;
            case "AirState":
                animator.CrossFade("air", .1f);
                break;
            case "SimpleJumpState":
                animator.CrossFade("air", .1f);
                break;
            case "CrouchState":
                animator.CrossFade("crouch", .1f);
                break;
            case "DashState":
                animator.CrossFade("dash", .01f);
                break;
        }
    }

    private void OnAnimatorIK(int layerIndex) 
    {
        //if(animator == null) return;

        if(ikRightTarget)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
            animator.SetIKPosition(AvatarIKGoal.RightHand, ikRightTarget.position);
        }

        if(ikLeftTarget)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);  
            animator.SetIKPosition(AvatarIKGoal.LeftHand, ikLeftTarget.position);
        }

        if(ikLookTarget)
        {
            animator.SetLookAtWeight(1, 0, 1, 0);
            animator.SetLookAtPosition(ikLookTarget.position);
        }
    }
}
