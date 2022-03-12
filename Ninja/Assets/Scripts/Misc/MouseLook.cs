using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Vector2 sensitivity = new Vector2(100f,80f);
    [SerializeField] private Transform playerBody;

    //[SerializeField] private LookState _lookState = LookState.TURN_BODY;

    float xRotation = 0f;

    [SerializeField] private InputState inputState;
    [SerializeField] private float maxRotZ = 1f;
    [SerializeField] private float rotSpeedZ = 2f;

    [SerializeField] private bool disabled;

    void Start()
    {
        UIManager.OnDisableMouse += Disable;
        UIManager.OnEnableMouse += Enable;
    }
    
    void LateUpdate()
    {
        if(disabled) return;

        // Get Input Rotations
        float mouseX = Input.GetAxis("Mouse X") * sensitivity.x;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity.y;

        // Invert
        xRotation -= mouseY;

        // Clamp
        xRotation = Mathf.Clamp(this.xRotation, -90f, 90f);

        // Movement Feedback
        Vector3 inputVector = inputState.AxisInput;

        // Lerp
        /*
        float zRotation = Mathf.Lerp(
            transform.localRotation.z, 
            (inputVector.x > 0)? -maxRotZ : (inputVector.x < 0) ? maxRotZ : 0f, 
            Mathf.Abs(inputVector.x) * rotSpeedZ);
        */

        float targetZRot = (inputVector.x > 0)? -maxRotZ : (inputVector.x < 0)? maxRotZ : 0f;
        float currentZ = transform.localRotation.eulerAngles.z;
        currentZ = (currentZ > 180)? currentZ - 360 : currentZ;
        float deltaZ = (targetZRot - currentZ) * rotSpeedZ * Time.deltaTime;
        float zRotation =  currentZ + deltaZ;

        if(currentZ > targetZRot)
            zRotation = Mathf.Max(zRotation, targetZRot);
        else if(currentZ < targetZRot)
            zRotation = Mathf.Min(zRotation, targetZRot);
        
        

        // Rotate Camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, zRotation);
        
        // Rotate transform
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void Enable()
    {
        disabled = false;
    }

    private void Disable()
    {
        disabled = true;
    }
}
