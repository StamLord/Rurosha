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
        this.xRotation -= mouseY;

        // Clamp
        this.xRotation = Mathf.Clamp(this.xRotation, -90f, 90f);

        // Movement Feedback
        Vector3 inputVector = inputState.AxisInput;
        float xRotation = Mathf.Lerp(
            transform.localRotation.z, 
            (inputVector.x > 0)? -maxRotZ : (inputVector.x < 0) ? maxRotZ : 0f, 
            Mathf.Abs(inputVector.x) * rotSpeedZ);

        // Rotate Camera
        transform.localRotation = Quaternion.Euler(this.xRotation, 0f, xRotation);
        
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
