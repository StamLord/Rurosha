using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour
{
    [SerializeField] private float kickForce = 5f;
    [SerializeField] private float kickUpwardsModifier = 1f;
    [SerializeField] private float kickRadius = 1f;
    [SerializeField] private Transform kickTransform;
    
    public void ActivateKick()
    {
        Collider[] colliders = Physics.OverlapSphere(kickTransform.position, kickRadius);

        foreach(Collider col in colliders)
        {
            Rigidbody rb = (col.transform != transform)? col.GetComponent<Rigidbody>() : null;

            if(rb)
                rb.AddExplosionForce(kickForce, kickTransform.position, kickForce, kickUpwardsModifier, ForceMode.VelocityChange);
        }
    }
}
