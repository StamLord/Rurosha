using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAnimator : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private string verticalParameter;
    [SerializeField] private string horizontalParameter;

    private void Update()
    {
        Vector3 velocity = navMeshAgent.transform.InverseTransformVector(navMeshAgent.velocity);

        animator.SetFloat(verticalParameter, velocity.z);
        animator.SetFloat(horizontalParameter, velocity.x);
    }
}