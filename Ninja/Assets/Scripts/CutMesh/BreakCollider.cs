using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakCollider : MonoBehaviour
{
    [SerializeField] [Min(3)] private int slicerAmount = 3;
    [SerializeField] private float timeBetweenSlicers = .2f;

    private Queue<GameObject> toSlice = new Queue<GameObject>();

    private bool isSlicing;

    private void OnTriggerEnter(Collider other) 
    {
        if(slicerAmount < 1) return;

        if(isSlicing) return;

        Sliceable sliceable = other.GetComponent<Sliceable>();
        toSlice.Enqueue(sliceable.gameObject);

        if(sliceable)
        {
            StartCoroutine(MultipleSliceCoroutine());
        }
    }

    private void Slice(Plane plane, GameObject sliceable)
    {
        GameObject[] slices = Slicer.Slice(plane, sliceable);

        Destroy(sliceable);

        foreach(GameObject go in slices)
        {
            toSlice.Enqueue(go);
            
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            if(rigidbody) rigidbody.AddForce(plane.normal, ForceMode.Impulse);
        }
    }

    IEnumerator MultipleSliceCoroutine()
    {
        isSlicing = true;

        float amount = (float)slicerAmount;
        for (var i = 0; i < slicerAmount; i++)
        {
            float angle = (i + 1) / amount * 360f;
            Vector3 normal = Quaternion.Euler(0, 0, angle) * Vector3.up;
            Plane plane = new Plane(normal, Vector3.zero);
            
            int count = toSlice.Count;
            Debug.Log("ToSlice: " + count);

            for(int j = 0; j < count; j++)
            {
                Debug.Log("Slice: " + i + " - " + j);
                GameObject go = toSlice.Dequeue();
                Slice(plane, go);             
                yield return new WaitForSeconds(timeBetweenSlicers);
            }
        }

        toSlice.Clear();
        isSlicing = false;
    }

    void OnDrawGizmos()
    {
        if(slicerAmount < 1) return;

        float amount = (float)slicerAmount;
        for (var i = 0; i < slicerAmount; i++)
        {
            float angle = (i + 1) / amount * 360f;
            Vector3 normal = Quaternion.Euler(0, 0, angle) * Vector3.up;
            Gizmos.DrawLine(transform.position - normal, transform.position + normal);
        }
    }
}
