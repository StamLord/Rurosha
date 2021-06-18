using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Move Behavior/Avoid Behavior")]
public class AvoidBehavior : MoveBehavior
{
    [SerializeField] private int numberOfRays;
    [SerializeField] private float angle;
    [SerializeField] private float range;
    [SerializeField] private LayerMask mask;

    public override Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null)
    {
        Vector3 move = Vector3.zero;

        float anglePerLoop = angle / (numberOfRays - 1);
        float halfAngle = angle / 2;

        for (var i = 0; i < numberOfRays; i++)
        {
            RaycastHit hitInfo;
            float yAngle = (anglePerLoop * i ) - halfAngle;
            Vector3 direction = Quaternion.Euler(0, yAngle, 0) * local.forward;

            Ray ray = new Ray(local.position, direction);
            
            if(Physics.Raycast(ray, out hitInfo, range, mask))
            {
               // Debug.Log(hitInfo.transform.name);
                move -= (1f / numberOfRays) * direction;
            }
            //else
            //    move += (1f / numberOfRays) * direction;
        }

        move.y = 0;

        return move;
    }
}
