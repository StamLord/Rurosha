using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool useRigidbody = true;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float maxVelocityChange = 10f;
    [SerializeField] private float minDistance = .2f;
    [SerializeField] private float probeRange= 1f;
    [SerializeField] private bool obstacleAvoid = false;
    [SerializeField] private Transform obstacleInPath;

    

    [Header("References")]
    [SerializeField] private Vector3 target;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private NavMeshObstacle obstacle;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Transform probeForward;
    [SerializeField] private Transform probeLeft;
    [SerializeField] private Transform probeRight;

    [Header("Debug")]
    [SerializeField] private bool debugView;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        rigidbody = GetComponent<Rigidbody>();

        if(useRigidbody)
        {
            rigidbody.freezeRotation = true;
            agent.updatePosition = false;
        }
        else
        {
            rigidbody.isKinematic = true;
        }

        target = transform.position;
    }

    void FixedUpdate()
    {
        if(useRigidbody == false || agent.pathPending) return;

        Vector3 targetVelocity;
        Vector3 velocityChange;

        if(Vector3.Distance(transform.position, target) < minDistance)
            targetVelocity = Vector3.zero;
        else
        {
            Vector3 direction = agent.nextPosition - transform.position;
            direction.Normalize();
            targetVelocity = direction * walkSpeed;
        }

        velocityChange = targetVelocity - rigidbody.velocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
        UpdateAgent();
    }

    void UpdateAgent()
    {
        obstacle.enabled = false;
        agent.SetDestination(target);
        obstacle.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        if(debugView == false) return;

        if(agent && agent.pathPending == false)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(agent.nextPosition, .5f);
            Gizmos.DrawLine(transform.position, transform.position + (agent.nextPosition - transform.position));
        }
    }
}
