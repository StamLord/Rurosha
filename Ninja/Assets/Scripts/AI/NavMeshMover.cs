using UnityEngine;
using UnityEngine.AI;

public class NavMeshMover : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform target;

    private void Update()
    {
        navMeshAgent.SetDestination(target.position);
    }
}
