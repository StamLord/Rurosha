using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PControls : MonoBehaviour
{
    [SerializeField] private InputState _inputState;

    [SerializeField] private TextMeshProUGUI _jumpVBText;
    [SerializeField] private TextMeshProUGUI _runVBText;
    [SerializeField] private TextMeshProUGUI _crouchVBText;
    [SerializeField] private TextMeshProUGUI _useVBText;
    
    void Update()
    {
        _inputState.AxisInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

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

    private void LateUpdate() 
    {
        _jumpVBText.text = "[" + _inputState.jump.State + "]";
        _runVBText.text = "[" + _inputState.run.State + "]";
        _crouchVBText.text = "[" + _inputState.crouch.State + "]";
        _useVBText.text = "[" + _inputState.use.State + "]";
        
    }
}

