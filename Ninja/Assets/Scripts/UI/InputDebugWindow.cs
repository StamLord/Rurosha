using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputDebugWindow : MonoBehaviour
{
    [SerializeField] private GameObject windowParent;
    [SerializeField] private InputState _inputState;

    [SerializeField] private TextMeshProUGUI _mb1VBText;
    [SerializeField] private TextMeshProUGUI _mb2VBText;
    [SerializeField] private TextMeshProUGUI _jumpVBText;
    [SerializeField] private TextMeshProUGUI _runVBText;
    [SerializeField] private TextMeshProUGUI _crouchVBText;
    [SerializeField] private TextMeshProUGUI _useVBText;

    [SerializeField] private TextMeshProUGUI _mb1VBTime;
    [SerializeField] private TextMeshProUGUI _mb2VBTime;
    [SerializeField] private TextMeshProUGUI _jumpVBTime;
    [SerializeField] private TextMeshProUGUI _runVBTime;
    [SerializeField] private TextMeshProUGUI _crouchVBTime;
    [SerializeField] private TextMeshProUGUI _useVBTime;

    [SerializeField] private TextMeshProUGUI _dbForwardText;
    [SerializeField] private TextMeshProUGUI _dbBackText;
    [SerializeField] private TextMeshProUGUI _dbLeftText;
    [SerializeField] private TextMeshProUGUI _dbRightText;

    public void Show(bool state)
    {
        windowParent.SetActive(state);
    }

    void LateUpdate()
    {
        if(windowParent.activeSelf == false) return;

        // States
        _mb1VBText.text = "[" + _inputState.MouseButton1.State + "]";
        _mb2VBText.text = "[" + _inputState.MouseButton2.State + "]";
        _jumpVBText.text = "[" + _inputState.Jump.State + "]";
        _runVBText.text = "[" + _inputState.Run.State + "]";
        _crouchVBText.text = "[" + _inputState.Crouch.State + "]";
        _useVBText.text = "[" + _inputState.Use.State + "]";
        
        // Press time
        _mb1VBTime.text = "[" + _inputState.MouseButton1.PressTime + "]";
        _mb2VBTime.text = "[" + _inputState.MouseButton2.PressTime + "]";
        _jumpVBTime.text = "" + _inputState.Jump.PressTime;
        _runVBTime.text = "" + _inputState.Run.PressTime;
        _crouchVBTime.text = "" + _inputState.Crouch.PressTime;
        _useVBTime.text = "" + _inputState.Use.PressTime;

        _dbForwardText.text = "[ " + _inputState.DoubleForward + "]";
        _dbBackText.text = "[ " + _inputState.DoubleBack + "]";
        _dbLeftText.text = "[ " + _inputState.DoubleLeft + "]";
        _dbRightText.text = "[ " + _inputState.DoubleRight + "]";;
    }
}
