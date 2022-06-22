using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class angleTest : MonoBehaviour
{
    public float angleX;
    public float angleY;

    public Transform target;

    private void Update()
    {
        // Get angle on x axis
        Vector3 dir = target.position - transform.position;
        Vector3 flatYDir = new Vector3(dir.x, 0, dir.z);

        Debug.DrawLine(transform.position, transform.position + dir, Color.red);
        Debug.DrawLine(transform.position, transform.position + flatYDir, Color.blue);

        angleX = Vector3.Angle(dir, flatYDir);
        
        // Get angle on Y axis - We flatten on y axis and get rotation around y axis
        Vector3 tPos = new Vector3(target.position.x, 0, target.position.z);
        Vector3 lPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 dir2 = tPos - lPos;
        angleY = Vector3.Angle(transform.forward, dir2);
    }

    private void OnDrawGizmos() 
    {
        Vector3 dir = (target.position - transform.position).normalized;
    }
}
