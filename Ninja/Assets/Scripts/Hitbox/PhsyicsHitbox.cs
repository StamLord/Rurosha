using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhsyicsHitbox : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float minVelocity = 1;
    [SerializeField] private float softDamageMult;
    [SerializeField] private float hardDamageMult;
    [SerializeField] private DamageType damageType;

    [Header("Debug Info")]
    [SerializeField] private Rigidbody rigidbody;

    private void OnValidate()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other) 
    {
        float speed = rigidbody.velocity.magnitude;
        
        // Do nothing if we're too slow
        if(speed < minVelocity)
            return;

        // Do nothing if collided with rigidbody who is faster than us
        if(other.rigidbody && other.rigidbody.velocity.magnitude > speed)
            return;
                
        // Scale by our mass
        float force = speed * rigidbody.mass;

        // Deal damage
        if(force > 0)
        {
            HurtboxDelegate hbd = other.collider.GetComponent<HurtboxDelegate>();
            if(hbd)
                hbd.Hit(null, Mathf.FloorToInt(force * softDamageMult), Mathf.FloorToInt(force * hardDamageMult), Vector3.up, damageType);
        }
    }
}

