using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayCollider : MonoBehaviour
{
    public Collider affectedCollider;
   
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if(rigid.velocity.y > 0)
            Physics.IgnoreCollision(other, affectedCollider, true);
        else
            Physics.IgnoreCollision(other, affectedCollider, false);
    }
}
