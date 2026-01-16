using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSlicer : MonoBehaviour
{
    [SerializeField] private List<GameObject> newSlices = new List<GameObject>();

    private void OnTriggerEnter(Collider other) 
    {
        // Ignore slices we already created
        if(newSlices.Contains(other.gameObject)) return;

        Sliceable sliceable = other.GetComponent<Sliceable>();
        if(sliceable)
        {
            StartCoroutine(SliceCoroutine(
                new Plane(transform.right, Vector3.zero), // Plane normal is transform.right so it will be vertical to object
                sliceable.gameObject));
        }
    }

    private void OnDisable() 
    {
        newSlices.Clear();        
    }

    private IEnumerator SliceCoroutine(Plane plane, GameObject toSlice)
    {
        GameObject[] slices = Slicer.Slice(plane, toSlice);

        // Add new sliced objects to a list of objects ignored in any further collisions
        foreach(GameObject s in slices)
            newSlices.Add(s);

        Destroy(toSlice);
        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        rigidbody.AddForce(plane.normal, ForceMode.Impulse);

        yield return null;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - transform.up, transform.position + transform.up);
    }
}
