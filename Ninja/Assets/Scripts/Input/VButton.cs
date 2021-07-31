using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VButtonState{UNPRESSED, PRESS_START, PRESSED, PRESS_END};

public class VButton
{
    private VButtonState _state;
    public VButtonState State {get {return _state;}}

    private float firstPress;
    private float lastPress;

    public bool Pressed { get { return (State == VButtonState.PRESS_START || State == VButtonState.PRESSED); }}
    public float PressTime { get { return ((State == VButtonState.UNPRESSED) ? 0f : lastPress - firstPress); }}

    public void Set(VButtonState newState)
    {
        _state = newState;
        
        switch(newState)
        {
            case VButtonState.PRESS_START:
                firstPress = Time.time;
                lastPress = Time.time;
                break;
            case VButtonState.PRESSED:
                lastPress = Time.time;
                break;
            case VButtonState.PRESS_END:
                lastPress = Time.time;
                break;
        }
    }
}
