using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Move Behavior/Follow Behavior")]
public class FollowBehavior : MoveBehavior
{
    [SerializeField] private float bufferZone = 1f;
    
    public override Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null)
    {
        Vector3 move = Vector3.zero;
        
        Vector3 distance = target.position - local.position;
        
        if(distance.sqrMagnitude > bufferZone * bufferZone)
            move = target.position - local.position;

        move.y = 0;

        return move;
    }
}
