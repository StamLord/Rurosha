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

    public void Set(VButtonState newState)
    {
        _state = newState;
    }

    // public void Press() 
    // {
    //     if (IsPressed() == VButtonState.UNPRESSED)
    //         firstPress = Time.time;

    //     lastPress = Time.time;

    // }

    // private VButtonState IsPressed()
    // {
    //     if(Time.time > lastPress)

    //         return VButtonState.UNPRESSED;

    //     if(Time.time == firstPress)
    //         return VButtonState.PRESS_START;

    //     if(Time.time == lastPress)
    //         return VButtonState.PRESS_END;

    //     return VButtonState.PRESSED;
    // }
}
