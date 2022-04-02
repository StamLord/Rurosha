using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeSensor : MonoBehaviour
{
    [Header("Ledge Info")]
    [SerializeField] bool ledgeDetected;
    [SerializeField] Vector3 ledgePoint;

    public bool LedgeDetected {get{return ledgeDetected;}}
    public Vector3 LedgePoint {get{return ledgePoint;}}

    [Space(20f)]

    [Header("Detection Settings")]
    [SerializeField] private float ledgeOffset;
    [SerializeField] private LayerMask ledgeMask;  

    [Space(20f)]

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private Color ledgeColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        DetectLedge();
    }

    private void DetectLedge()
    {
        RaycastHit ledge;

        // Make sure we don't face a wall
        if(Physics.Raycast(transform.position + Vector3.up - Vector3.up * .1f, transform.forward, ledgeOffset, ledgeMask))
            return;

        Ray ledgeRay = new Ray(transform.position + transform.forward * ledgeOffset + Vector3.up - Vector3.up * .1f, Vector3.down);
        
        ledgeDetected = Physics.Raycast(ledgeRay, out ledge, 1f, ledgeMask);
        ledgePoint = ledge.point;
    }

    private void OnDrawGizmos() 
    {
        if(debugView == false || ledgeDetected == false) return;

        Gizmos.color = ledgeColor;
        Gizmos.DrawCube(ledgePoint, new Vector3(.1f, .1f, .1f));
    }
}
