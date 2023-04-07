using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputState : MonoBehaviour
{
    // Movement Axis
    [SerializeField] private Vector3 _axisInput;
    public Vector3 AxisInput { get { return _axisInput;} set {_axisInput = Vector3.ClampMagnitude(value, 1f);}}

    // Mouse Scroll Axis
    [SerializeField] private float _scrollInput;
    public float ScrollInput { get { return _scrollInput;} set {_scrollInput = value;}}

    public bool DoubleForward, DoubleBack, DoubleLeft, DoubleRight;
    private bool prevDoubleForward, prevDoubleBack, prevDoubleLeft, prevDoubleRight;

    public float Rotation;

    public VButton Jump = new VButton();
    public VButton Run = new VButton();
    public VButton Crouch = new VButton();
    public VButton Use = new VButton();
    public VButton Defend = new VButton();
    public VButton Kick = new VButton();
    public VButton Spell = new VButton();
    public VButton Sit = new VButton();
    public VButton Draw = new VButton();
    public VButton Drop = new VButton();
    
    public VButton MouseButton1 = new VButton();
    public VButton MouseButton2 = new VButton();

    public VButton Num1 = new VButton();
    public VButton Num2 = new VButton();
    public VButton Num3 = new VButton();
    public VButton Num4 = new VButton();
    public VButton Num5 = new VButton();
    public VButton Num6 = new VButton();
    public VButton Num7 = new VButton();
    public VButton Num8 = new VButton();
    public VButton Num9 = new VButton();
    public VButton Num0 = new VButton();

    [SerializeField] private bool debug;

    private void Update() 
    {
        ResetDoubleAxisFlags();        
    }

    private void ResetDoubleAxisFlags()
    {
        if(prevDoubleForward)
            DoubleForward = false;

        if(prevDoubleBack)
            DoubleBack = false;

        if(prevDoubleLeft)
            DoubleLeft = false;

        if(prevDoubleRight)
            DoubleRight = false;

        prevDoubleForward = DoubleForward;
        prevDoubleBack = DoubleBack;
        prevDoubleLeft = DoubleLeft;
        prevDoubleRight = DoubleRight;
    }

    public void ResetInput()
    {
        AxisInput = Vector3.zero;

        Jump.Set(VButtonState.UNPRESSED);
        Run.Set(VButtonState.UNPRESSED);
        Crouch.Set(VButtonState.UNPRESSED);
        Use.Set(VButtonState.UNPRESSED);
        Defend.Set(VButtonState.UNPRESSED);
        Kick.Set(VButtonState.UNPRESSED);
        Spell.Set(VButtonState.UNPRESSED);
        Sit.Set(VButtonState.UNPRESSED);
        Draw.Set(VButtonState.UNPRESSED);
        Drop.Set(VButtonState.UNPRESSED);
        
        MouseButton1.Set(VButtonState.UNPRESSED);
        MouseButton2.Set(VButtonState.UNPRESSED);

        Num1.Set(VButtonState.UNPRESSED);
        Num2.Set(VButtonState.UNPRESSED);
        Num3.Set(VButtonState.UNPRESSED);
        Num4.Set(VButtonState.UNPRESSED);
        Num5.Set(VButtonState.UNPRESSED);
        Num6.Set(VButtonState.UNPRESSED);
        Num7.Set(VButtonState.UNPRESSED);
        Num8.Set(VButtonState.UNPRESSED);
        Num9.Set(VButtonState.UNPRESSED);
        Num0.Set(VButtonState.UNPRESSED);
    }

    private void OnDrawGizmos() 
    {
        if(!debug) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, AxisInput);
    }
}
