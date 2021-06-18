using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputState : MonoBehaviour
{
    [SerializeField] private Vector3 _axisInput;
    public Vector3 AxisInput { get { return _axisInput;} set {_axisInput = Vector3.ClampMagnitude(value, 1f);}}
    public float rotation;

    public VButton jump = new VButton();
    public VButton run = new VButton();
    public VButton crouch = new VButton();
    public VButton use = new VButton();

    public bool debug;

    private void OnDrawGizmos() 
    {
        if(!debug) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, AxisInput);
    }

}
