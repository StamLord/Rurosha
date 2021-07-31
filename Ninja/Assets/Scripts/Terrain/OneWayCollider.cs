using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayCollider : MonoBehaviour
{
    public Collider affectedCollider;
   
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if(rigid == null) return;
        
        if(rigid.velocity.y > 0)
        {
            Debug.Log("Ignoring " + other);
            Physics.IgnoreCollision(other, affectedCollider, true);
        }
        
    }

    void OnTriggerExit(Collider other) 
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if(rigid == null) return;
        
        Debug.Log("UnIgnoring " + other);
        Physics.IgnoreCollision(other, affectedCollider, false);
    }
}
