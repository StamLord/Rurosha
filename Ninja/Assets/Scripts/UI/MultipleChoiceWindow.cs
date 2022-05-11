using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceWindow : UIWindow
{
    private enum InputDirection {VERTICAL, HORIZONTAL, BOTH};

    [Header("References")]
    [SerializeField] private Button[] choices;

    [Header("Selection")]
    [SerializeField] private InputDirection inputDirection;
    [SerializeField] private bool inverseDirection;
    [SerializeField] private float inputInterval = .1f; 

    [Header("Real Time Data")]
    [SerializeField] private int selection = 0;
    [SerializeField] private float lastInput;

    public override void ProcessInput(Vector3 axis, bool select)
    {
        if(select)
        {
            Select(selection);
            return;
        }

        if(Time.time < lastInput + inputInterval)
            return;
        
        int dir = (inverseDirection)? -1 : 1;
        switch(inputDirection)
        {
            case InputDirection.VERTICAL:
                if(axis.z > 0)
                {
                    UpdateSelection(selection + 1 * dir);
                    lastInput = Time.time;
                }
                else if(axis.z < 0)
                {
                    UpdateSelection(selection - 1 * dir);
                    lastInput = Time.time;
                }
                break;
            case InputDirection.HORIZONTAL:
                if(axis.x > 0)
                {
                    UpdateSelection(selection + 1 * dir);
                    lastInput = Time.time;
                }
                else if(axis.x < 0)
                {
                    UpdateSelection(selection - 1 * dir);
                    lastInput = Time.time;
                }
                break;
        }
    }

    private void UpdateSelection(int value)
    {
        selection = Mathf.Clamp(value, 0, choices.Length - 1);
        choices[selection].Select();
    }

    public override bool Select(int index)
    {
        if(index < 0 || index >= choices.Length)
            return false;

        choices[selection].onClick.Invoke();
        return true;
    }
}
