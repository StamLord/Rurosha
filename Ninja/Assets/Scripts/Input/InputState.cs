using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputState : MonoBehaviour
{
    [SerializeField] private Vector3 _axisInput;
    public Vector3 AxisInput { get { return _axisInput;} set {_axisInput = Vector3.ClampMagnitude(value, 1f);}}

    public bool DoubleForward, DoubleBack, DoubleLeft, DoubleRight;
    private bool prevDoubleForward, prevDoubleBack, prevDoubleLeft, prevDoubleRight;

    public float Rotation;

    public VButton Jump = new VButton();
    public VButton Run = new VButton();
    public VButton Crouch = new VButton();
    public VButton Use = new VButton();
    public VButton Defend = new VButton();
    public VButton Kick = new VButton();
    
    public VButton MouseButton1 = new VButton();
    public VButton MouseButton2 = new VButton();

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

    private void OnDrawGizmos() 
    {
        if(!debug) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, AxisInput);
    }
}
