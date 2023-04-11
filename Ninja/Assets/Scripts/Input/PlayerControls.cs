using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private InputState inputState;
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

    private void Update()
    {
        if(movementDisabled == false)
        {   
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                        
            inputState.AxisInput = (useRawInput)? rawInput : input;

            DoubleInputCheck(rawInput);

            // Jump
            UpdateVirtualButton("Jump", inputState.Jump);

            //Run
            UpdateVirtualButton("Run", inputState.Run);

            // Crouch
            UpdateVirtualButton("Crouch", inputState.Crouch);

            // Defend
            UpdateVirtualButton("Defend", inputState.Defend);

            // Kick
            UpdateVirtualButton("Kick", inputState.Kick);

            // Spell
            UpdateVirtualButton("Spell", inputState.Spell);

            // Sit
            UpdateVirtualButton("Sit", inputState.Sit);

            // Draw
            UpdateVirtualButton("Draw", inputState.Draw);

            // Draw
            UpdateVirtualButton("Drop", inputState.Drop);
        }

        if(interactionDisabled == false)
        {
            // Use
            UpdateVirtualButton("Use", inputState.Use);

            // Alpha numerics
            UpdateVirtualButton("Alpha1", inputState.Num1);
            UpdateVirtualButton("Alpha2", inputState.Num2);
            UpdateVirtualButton("Alpha3", inputState.Num3);
            UpdateVirtualButton("Alpha4", inputState.Num4);
            UpdateVirtualButton("Alpha5", inputState.Num5);
            UpdateVirtualButton("Alpha6", inputState.Num6);
            UpdateVirtualButton("Alpha7", inputState.Num7);
            UpdateVirtualButton("Alpha8", inputState.Num8);
            UpdateVirtualButton("Alpha9", inputState.Num9);
            UpdateVirtualButton("Alpha0", inputState.Num0);
        }

        if(mouseDisabled == false)
        {
            // Mouse Button 1
            UpdateVirtualButton("Fire1", inputState.MouseButton1);

            // Mouse Button 2
            UpdateVirtualButton("Fire2", inputState.MouseButton2);

            // Mouse Scroll
            inputState.ScrollInput = Input.GetAxis("Mouse ScrollWheel");
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
                    inputState.DoubleForward = true;
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
                    inputState.DoubleBack = true;
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
                    inputState.DoubleLeft = true;
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
                    inputState.DoubleRight = true;
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
        inputState.ResetInput();
        movementDisabled = true;
    }

    public void EnableMovement()
    {
        movementDisabled = false;
    }

    public void DisableInteraction()
    {
        interactionDisabled = true;
        inputState.Use.Set(VButtonState.UNPRESSED);
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
