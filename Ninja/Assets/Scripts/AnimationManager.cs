using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RigidbodyFPSWalker fpsWalker;

    void LateUpdate()
    {
        if(animator)
        {
            animator.SetFloat("Speed X", fpsWalker.Speed.x);
            animator.SetFloat("Speed Z", fpsWalker.Speed.z);
            animator.SetBool("Crouching", fpsWalker.Crouching);
        }
    }
}
