using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    
    public float maxSpeed = 10;
    public float trueMaxSpeed;
    public float maxAccel;

    public float orientation;
    public float rotation;
    public Vector3 velocity;
    protected Steering steer;

    public float maxRotation = 45f;
    public float maxAngularAccel = 45f;

    private void Start() 
    {
        velocity = Vector3.zero;
        steer = new Steering();
        trueMaxSpeed = maxSpeed;    
    }

    public void SetSteering(Steering steering, float weight)
    {
        this.steer.linear += (weight * steer.linear);
        this.steer.angular += (weight * steer.angular);
    }

    void Update() 
    {
        Vector3 displacement = velocity * Time.deltaTime;
        displacement.y = 0;

        orientation += rotation * Time.deltaTime;

        if(orientation < 0f)
            orientation += 360f;
        else if(orientation > 360f)
            orientation -= 360f;

        transform.Translate(displacement, Space.World);
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, orientation);
    }

    void LateUpdate()
    {
        velocity += steer.linear * Time.deltaTime;   
        rotation += steer.angular * Time.deltaTime;

        if(velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity = velocity * maxSpeed;
        }

        if(steer.linear.magnitude == 0f)
        {
            velocity = Vector3.zero;
        }

        steer = new Steering();
    }

    void SpeedReset()
    {
        maxSpeed = trueMaxSpeed;
    }

}
