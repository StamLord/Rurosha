using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedTransform : MonoBehaviour
{
    public Transform parent;
    public Vector3 offset;
    public Quaternion initialRotation;
    public float posSpeed = 10f;
    public float rotSpeed = 10f;

    void Start()
    {
        GameObject dynamicParent = GameObject.Find("_Dynamic");
        parent = transform.parent;
        transform.SetParent(dynamicParent? dynamicParent.transform : null);
        offset = parent.position - transform.position;
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        float sqrDistance = (parent.position - offset - transform.position).sqrMagnitude;
        float newSpeed = Mathf.Max(posSpeed, sqrDistance * sqrDistance);

        Vector3 targetPosition = parent.position - (parent.rotation * offset);
        Debug.DrawLine(parent.position, targetPosition);
        transform.position = Vector3.Lerp(transform.position, targetPosition, newSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, parent.rotation, rotSpeed * Time.deltaTime); 
    }
}

