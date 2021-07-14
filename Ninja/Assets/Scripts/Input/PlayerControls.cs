using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private InputState _inputState;
    [SerializeField] private float doubleTapWindow = .35f;

    private float lastForwardTime = -1f;
    private bool doubleForwardReady;
    private bool movingForward;

    private float lastBackTime = -1f;
    private bool doubleBackReady;
    private bool movingBack;

    private float lastLeftTime = -1f;
    private bool doubleLeftReady;
    private bool movingLeft;

    private float lastRightTime = -1f;
    private bool doubleRightReady;
    private bool movingRight;

    [SerializeField] private bool movementDisabled;
    [SerializeField] private bool interactionDisabled;

    void Start() 
    {
        UIManager.OnDisableMovement += DisableMovement;
        UIManager.OnEnableMovement += EnableMovement;
        UIManager.OnEnableInteraction += EnableInteraction;
        UIManager.OnDisableInteraction += DisableInteraction;
    }

    void Update()
    {
        if(movementDisabled == false)
        {
            _inputState.AxisInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            DoubleInputCheck(rawInput);

            //Jump
            if(Input.GetButtonDown("Jump"))
                _inputState.jump.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Jump"))
                _inputState.jump.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Jump"))
                _inputState.jump.Set(VButtonState.PRESSED);
            else
                _inputState.jump.Set(VButtonState.UNPRESSED);

            //Run
            if(Input.GetButtonDown("Run"))
                _inputState.run.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Run"))
                _inputState.run.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Run"))
                _inputState.run.Set(VButtonState.PRESSED);
            else
                _inputState.run.Set(VButtonState.UNPRESSED);

            //Crouch
            if(Input.GetButtonDown("Crouch"))
                _inputState.crouch.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Crouch"))
                _inputState.crouch.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Crouch"))
                _inputState.crouch.Set(VButtonState.PRESSED);
            else
                _inputState.crouch.Set(VButtonState.UNPRESSED);
        }

        if(interactionDisabled == false)
        {
            //Use
            if(Input.GetButtonDown("Use"))
                _inputState.use.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Use"))
                _inputState.use.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Use"))
                _inputState.use.Set(VButtonState.PRESSED);
            else
                _inputState.use.Set(VButtonState.UNPRESSED);
        }
    }

    private void DoubleInputCheck(Vector3 rawInput)
    {
        if(rawInput.z == 0)
            movingForward = movingBack = false;

        if(rawInput.x == 0) 
            movingLeft = movingRight = false;
        
        ResetDoubleInputReady();

        // Forward
        if(rawInput.z == 1)
        {
            if(movingForward == false)
            {
                movingForward = true;
                lastForwardTime = Time.time;

                if(doubleForwardReady)
                {
                    doubleForwardReady = false;
                    _inputState.doubleForward = true;
                }
                else
                    doubleForwardReady = true;
            }
        }

        // Back
        if(rawInput.z == -1)
        {
            if(movingBack == false)
            {
                movingBack = true;
                lastBackTime = Time.time;

                if(doubleBackReady)
                {
                    doubleBackReady = false;
                    _inputState.doubleBack = true;
                }
                else
                    doubleBackReady = true;
            }
        }

        // Left
        if(rawInput.x == -1)
        {
            if(movingLeft == false)
            {
                movingLeft = true;
                lastLeftTime = Time.time;

                if(doubleLeftReady)
                {
                    doubleLeftReady = false;
                    _inputState.doubleLeft = true;
                }
                else
                    doubleLeftReady = true;
            }
        }

        // Right
        if(rawInput.x == 1)
        {
            if(movingRight == false)
            {
                movingRight = true;
                lastRightTime = Time.time;

                if(doubleRightReady)
                {
                    doubleRightReady = false;
                    _inputState.doubleRight = true;
                }
                else
                    doubleRightReady = true;
            }
        }
    }

    private void ResetDoubleInputReady()
    {
        if(Time.time > lastForwardTime + doubleTapWindow)
            doubleForwardReady = false;

        if(Time.time > lastBackTime + doubleTapWindow)
            doubleBackReady = false;

        if(Time.time > lastLeftTime + doubleTapWindow)
            doubleLeftReady = false;

        if(Time.time > lastRightTime + doubleTapWindow)
            doubleRightReady = false;
    }

    private void DisableMovement()
    {
        movementDisabled = true;
    }

    private void EnableMovement()
    {
        movementDisabled = false;
    }

    private void DisableInteraction()
    {
        interactionDisabled = true;
        _inputState.use.Set(VButtonState.UNPRESSED);
    }

    private void EnableInteraction()
    {
        interactionDisabled = false;
    }
}

