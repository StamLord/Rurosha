using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMBInput : MonoBehaviour
{
    public InputState inputState;
    public MoveBehavior moveBehavior;
    public Transform target;

    private List<Transform> cachedNeighbors = new List<Transform>();

    // Update is called once per frame
    void Update()
    {
        if(cachedNeighbors == null) cachedNeighbors = new List<Transform>();

        inputState.AxisInput = moveBehavior.CalculateMove(transform, target, cachedNeighbors);
        cachedNeighbors = GetNearbyFlockMembers();
    }

    private List<Transform> GetNearbyFlockMembers()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        List<Transform> neighbors = new List<Transform>();

        foreach(Collider c in colliders)
        {
            BTBrain btb = c.GetComponent<BTBrain>();
            if(btb)
                neighbors.Add(btb.transform);
        }
        
        return neighbors;
    }
}
