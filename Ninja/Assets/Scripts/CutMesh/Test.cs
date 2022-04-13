using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject toSlice;

    void Start()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        GameObject[] slices = Slicer.Slice(plane, toSlice);
        Destroy(toSlice);
        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
    }
}
