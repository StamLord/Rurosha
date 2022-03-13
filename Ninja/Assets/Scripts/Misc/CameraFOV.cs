using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private new Rigidbody rigidbody;

    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    [SerializeField] private float minFov;
    [SerializeField] private float maxFov;

    [SerializeField] [Range(0,1)] private float transitionSpeed = .1f;

    [SerializeField] private bool ignoreY;

    private float sqrMinSpeed;
    private float sqrMaxSpeed;

    void Start() 
    {   
        sqrMinSpeed = minSpeed * minSpeed;    
        sqrMaxSpeed = maxSpeed * maxSpeed;    
    }

    void Update()
    {
        if(camera == null)
            return;
        
        Vector3 velocity = rigidbody.velocity;
        
        if(ignoreY)
            velocity.y = 0;

        float targetFov = Mathf.Lerp(minFov, maxFov, velocity.sqrMagnitude / sqrMaxSpeed);
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, transitionSpeed);
    }
}
