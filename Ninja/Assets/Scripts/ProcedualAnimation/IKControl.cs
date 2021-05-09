using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform rightHandObj;

    void OnAnimatorIK(int layerIndex)
    {
        if(animator) 
        {
            if(rightHandObj != null) 
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
                animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
            }
            else 
            {          
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0); 
                animator.SetLookAtWeight(0);
            }       
        }
    }
}
