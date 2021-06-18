using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Move Behavior/Composite Move Behavior")]
public class CompositeMoveBehavior : MoveBehavior
{
    [SerializeField] private MoveBehavior[] behaviors;
    [SerializeField] private float[] weights;

    public override Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null)
    {
        if(behaviors.Length != weights.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        Vector3 move = Vector3.zero;

        for(int i = 0; i < behaviors.Length; i++)
        {
            Vector3 partialMove = behaviors[i].CalculateMove(local, target, flock) * weights[i];
            // Debug.Log(behaviors[i].name + " Vector: " + partialMove);
            if(partialMove != Vector3.zero)
            {
                if(partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}
