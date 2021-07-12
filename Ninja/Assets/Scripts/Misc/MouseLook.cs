using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Vector2 sensitivity = new Vector2(100f,80f);
    [SerializeField] private Transform playerBody;

    public enum LookState {TURN_BODY, TURN_HEAD}
    [SerializeField] private LookState _lookState = LookState.TURN_BODY;

    float xRotation = 0f;

    [SerializeField] private bool disabled;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        UIManager.OnDisableMouse += Disable;
        UIManager.OnEnableMouse += Enable;
    }
    
    void LateUpdate()
    {
        if(disabled) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity.x;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity.y;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        switch(_lookState)
        {
            case LookState.TURN_BODY:
            playerBody.Rotate(Vector3.up * mouseX);
            break;

            case LookState.TURN_HEAD:
            break;
        }
    }

    void SetLookState(LookState lookState)
    {
        _lookState = lookState;
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
