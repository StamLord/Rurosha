using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBody : MonoBehaviour
{
    [SerializeField] private float _buoyancy = 10f;
    [SerializeField] private float _drag = 1f;
    [SerializeField] private List<Rigidbody> _bodiesInWater = new List<Rigidbody>();
    [SerializeField] private float depth;
    [SerializeField] private float surfaceHeight;

    private void Start() 
    {
        depth = transform.lossyScale.y;
        surfaceHeight = transform.position.y + depth * .5f;
    }
    
    void FixedUpdate()
    {
        foreach(Rigidbody rb in _bodiesInWater)
        {
            Vector3 scale = rb.transform.lossyScale;
            float displacementAmount = (scale.x * scale.y * 2) + (scale.z * scale.y * 2) + (scale.x * scale.z * 2); // AKA cube volume
            float submerged = Mathf.Clamp01(surfaceHeight - rb.position.y + scale.y * .5f/ scale.y); // 0 is outside of water 1 is fully submerged

            rb.AddForce(Vector3.up * _buoyancy * submerged, ForceMode.Acceleration);
            rb.drag = _drag * submerged;
            rb.angularDrag = _drag * submerged;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        Rigidbody rb = other.attachedRigidbody;
        if(rb && _bodiesInWater.Contains(rb) == false)
            _bodiesInWater.Add(rb);
    }

    private void OnTriggerExit(Collider other) 
    {
        Rigidbody rb = other.attachedRigidbody;
        if(rb && _bodiesInWater.Contains(rb))
            _bodiesInWater.Remove(rb);    
    }
}
