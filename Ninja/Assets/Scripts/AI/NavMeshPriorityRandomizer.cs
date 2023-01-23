using UnityEngine;
using UnityEngine.AI;

public class NavMeshPriorityRandomizer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private void Start()
    {
        agent.avoidancePriority = Random.Range(0, 99);
    }
}
