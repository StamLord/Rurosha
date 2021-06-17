using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSteerBehavior : MonoBehaviour
{
    public float weight = 1f;

    public GameObject target;
    public SteeringAgent steeringAgent;
    public Vector3 dest;

    public float maxSpeed = 50f;
    public float maxAccel = 50f;
    public float maxRotation = 5f;
    public float maxAngularAccel = 5f;

    public virtual void Start() 
    {
        steeringAgent = transform.GetComponent<SteeringAgent>();    
    }

    public virtual void Update() 
    {
        steeringAgent.SetSteering(GetSteering(), weight);
    }
    
    public virtual Steering GetSteering()
    {
        return new Steering();
    }
}
