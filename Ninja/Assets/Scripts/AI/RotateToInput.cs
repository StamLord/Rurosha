using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToInput : MonoBehaviour
{
    [SerializeField] private InputState inputState;
    [SerializeField] private float turnSpeed = .1f;

    void LateUpdate()
    {
        Vector3 inputVector = inputState.AxisInput;
        if(inputVector.magnitude > 0)
            transform.forward = Vector3.Lerp(transform.forward, inputState.AxisInput, turnSpeed);
    }
}
