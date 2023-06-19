using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedTransform : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private float posSpeed = 10f;
    [SerializeField] private float rotSpeed = 10f;

    private Vector3 offset;
    private Quaternion initialRotation;

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
        transform.position = Vector3.Lerp(transform.position, targetPosition, posSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, parent.rotation, rotSpeed * Time.deltaTime); 
    }
}

