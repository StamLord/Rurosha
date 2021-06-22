using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputDebugWindow : MonoBehaviour
{
    [SerializeField] private GameObject windowParent;
    [SerializeField] private InputState _inputState;

    [SerializeField] private TextMeshProUGUI _jumpVBText;
    [SerializeField] private TextMeshProUGUI _runVBText;
    [SerializeField] private TextMeshProUGUI _crouchVBText;
    [SerializeField] private TextMeshProUGUI _useVBText;

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
        _jumpVBText.text = "[" + _inputState.jump.State + "]";
        _runVBText.text = "[" + _inputState.run.State + "]";
        _crouchVBText.text = "[" + _inputState.crouch.State + "]";
        _useVBText.text = "[" + _inputState.use.State + "]";

        _jumpVBTime.text = "" + _inputState.jump.PressTime;
        _runVBTime.text = "" + _inputState.run.PressTime;
        _crouchVBTime.text = "" + _inputState.crouch.PressTime;
        _useVBTime.text = "" + _inputState.use.PressTime;

        _dbForwardText.text = "[ " + _inputState.doubleForward + "]";
        _dbBackText.text = "[ " + _inputState.doubleBack + "]";
        _dbLeftText.text = "[ " + _inputState.doubleLeft + "]";
        _dbRightText.text = "[ " + _inputState.doubleRight + "]";;
    }
}
