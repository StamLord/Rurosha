using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamingState : MonoBehaviour
{
    [SerializeField] private EnemyController controller;
    [SerializeField] private Transform[] checkPoints;
    [SerializeField] private int nextCheckpoint;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float defaultWaitAtPoint = 1f;
    [SerializeField] private float[] waitAtPoints;
    [SerializeField] private bool pointArrived;
    [SerializeField] private float pointArrivalTime;

    void Start()
    {
        controller = GetComponent<EnemyController>();
    }

    void Update()
    {
        if(checkPoints[nextCheckpoint] == null) return;

        float timeToWait = defaultWaitAtPoint;
        if(waitAtPoints.Length > nextCheckpoint)    
            timeToWait = waitAtPoints[nextCheckpoint];
            
        if(Vector3.Distance(transform.position, checkPoints[nextCheckpoint].position) <= minDistance)
        {
            if(pointArrived)
            {
                if((Time.time - pointArrivalTime) > timeToWait)
                {
                    // Advance to next point after waiting to required time
                    nextCheckpoint++;
                    nextCheckpoint %= checkPoints.Length;
                    pointArrived = false;
                }
            }
            else
            {
                pointArrived = true;
                pointArrivalTime = Time.time;
            }
        }

        // Update controller with target
        controller.SetTarget(checkPoints[nextCheckpoint].position);
    }

    void OnDrawGizmosSelected()
    {
        Color color = Color.red;
        for(int i =0; i < checkPoints.Length; i++)
        {
            if(i == nextCheckpoint) color.a = 1f; // Active Checkpoint, Red
            else color.a = .25f; // Non Active Checkpoint, Ghost Red

            Gizmos.color = color;
            Gizmos.DrawSphere(checkPoints[i].position, .5f);
        }
    }
}
