using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralSpin : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int axis = 1;

    void Update()
    {
        Vector3 vAxis = (axis == 0) ? transform.forward : (axis == 1) ? transform.right : transform.up;
        transform.RotateAround(vAxis, speed);
    }
}
