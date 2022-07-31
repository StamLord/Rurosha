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
    [SerializeField] private bool mouseDisabled;

    void Update()
    {
        if(movementDisabled == false)
        {   
            _inputState.AxisInput = (useRawInput)? new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) : new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) ;
            Vector3 rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            DoubleInputCheck(rawInput);

            // Jump
            UpdateVirtualButton("Jump", _inputState.Jump);

            //Run
            UpdateVirtualButton("Run", _inputState.Run);

            // Crouch
            UpdateVirtualButton("Crouch", _inputState.Crouch);

            // Defend
            UpdateVirtualButton("Defend", _inputState.Defend);

            // Kick
            UpdateVirtualButton("Kick", _inputState.Kick);

            // Spell
            UpdateVirtualButton("Spell", _inputState.Spell);

            // Sit
            UpdateVirtualButton("Sit", _inputState.Sit);

            // Draw
            UpdateVirtualButton("Draw", _inputState.Draw);

            // Draw
            UpdateVirtualButton("Drop", _inputState.Drop);
        }

        if(interactionDisabled == false)
        {
            // Use
            UpdateVirtualButton("Use", _inputState.Use);
        
            

            // Alpha numerics
            UpdateVirtualButton("Alpha1", _inputState.Num1);
            UpdateVirtualButton("Alpha2", _inputState.Num2);
            UpdateVirtualButton("Alpha3", _inputState.Num3);
            UpdateVirtualButton("Alpha4", _inputState.Num4);
            UpdateVirtualButton("Alpha5", _inputState.Num5);
            UpdateVirtualButton("Alpha6", _inputState.Num6);
            UpdateVirtualButton("Alpha7", _inputState.Num7);
            UpdateVirtualButton("Alpha8", _inputState.Num8);
            UpdateVirtualButton("Alpha9", _inputState.Num9);
            UpdateVirtualButton("Alpha0", _inputState.Num0);
        }

        if(mouseDisabled == false)
        {
            // Mouse Button 1
            UpdateVirtualButton("Fire1", _inputState.MouseButton1);

            // Mouse Button 2
            UpdateVirtualButton("Fire2", _inputState.MouseButton2);

            // Mouse Scroll
            _inputState.ScrollInput = Input.GetAxis("Mouse ScrollWheel");
        }
    }

    private void UpdateVirtualButton(string unityButtonName, VButton virtualButton)
    {
        if(Input.GetButtonDown(unityButtonName))
            virtualButton.Set(VButtonState.PRESS_START);
        else if (Input.GetButtonUp(unityButtonName))
            virtualButton.Set(VButtonState.PRESS_END);
        else if (Input.GetButton(unityButtonName))
            virtualButton.Set(VButtonState.PRESSED);
        else
            virtualButton.Set(VButtonState.UNPRESSED);
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

    public void DisableMovement()
    {
        movementDisabled = true;
    }

    public void EnableMovement()
    {
        movementDisabled = false;
    }

    public void DisableInteraction()
    {
        interactionDisabled = true;
        _inputState.Use.Set(VButtonState.UNPRESSED);
    }

    public void EnableInteraction()
    {
        interactionDisabled = false;
    }

    public void EnableMouse()
    {
        mouseDisabled = false;
    }

    public void DisableMouse()
    {
        mouseDisabled = true;
    }
}

