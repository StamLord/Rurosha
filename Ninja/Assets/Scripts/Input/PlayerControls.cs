using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private InputState _inputState;
    [SerializeField] private bool useRawInput = true;
    [SerializeField] private float doubleTapWindow = .35f;

    #region Double Tap

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

    #endregion

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
            _inputState.AxisInput = (useRawInput)? new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) : new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) ;
            Vector3 rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            DoubleInputCheck(rawInput);

            //Jump
            if(Input.GetButtonDown("Jump"))
                _inputState.Jump.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Jump"))
                _inputState.Jump.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Jump"))
                _inputState.Jump.Set(VButtonState.PRESSED);
            else
                _inputState.Jump.Set(VButtonState.UNPRESSED);

            //Run
            if(Input.GetButtonDown("Run"))
                _inputState.Run.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Run"))
                _inputState.Run.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Run"))
                _inputState.Run.Set(VButtonState.PRESSED);
            else
                _inputState.Run.Set(VButtonState.UNPRESSED);

            //Crouch
            if(Input.GetButtonDown("Crouch"))
                _inputState.Crouch.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Crouch"))
                _inputState.Crouch.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Crouch"))
                _inputState.Crouch.Set(VButtonState.PRESSED);
            else
                _inputState.Crouch.Set(VButtonState.UNPRESSED);
        }

        if(interactionDisabled == false)
        {
            //Use
            if(Input.GetButtonDown("Use"))
                _inputState.Use.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Use"))
                _inputState.Use.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Use"))
                _inputState.Use.Set(VButtonState.PRESSED);
            else
                _inputState.Use.Set(VButtonState.UNPRESSED);
        }

        if(interactionDisabled == false)
        {
            //Mouse Button 1
            if(Input.GetButtonDown("Fire1"))
                _inputState.MouseButton1.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Fire1"))
                _inputState.MouseButton1.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Fire1"))
                _inputState.MouseButton1.Set(VButtonState.PRESSED);
            else
                _inputState.MouseButton1.Set(VButtonState.UNPRESSED);

            //Mouse Button 2
            if(Input.GetButtonDown("Fire2"))
                _inputState.MouseButton2.Set(VButtonState.PRESS_START);
            else if (Input.GetButtonUp("Fire2"))
                _inputState.MouseButton2.Set(VButtonState.PRESS_END);
            else if (Input.GetButton("Fire2"))
                _inputState.MouseButton2.Set(VButtonState.PRESSED);
            else
                _inputState.MouseButton2.Set(VButtonState.UNPRESSED);
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
                    _inputState.DoubleForward = true;
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
                    _inputState.DoubleBack = true;
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
                    _inputState.DoubleLeft = true;
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
                    _inputState.DoubleRight = true;
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
        _inputState.Use.Set(VButtonState.UNPRESSED);
    }

    private void EnableInteraction()
    {
        interactionDisabled = false;
    }
}

