using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
    [SerializeField] private bool _isSolid = true;

    [SerializeField] private Material _insideMaterial;

    [SerializeField] private bool _reverseWindTriangles = false;

    [SerializeField] private bool _useGravity = false;

    [SerializeField] private bool _shareVertices = false;

    [SerializeField] private bool _smoothVertices = false;

    private Vector3 minMeshDimensions = new Vector3(.1f, .1f, .1f);
    private MeshFilter meshFilter;

    private new Rigidbody rigidbody;
    public Rigidbody Rigidbody { get {return rigidbody;}}

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        rigidbody = GetComponent<Rigidbody>();
    }
    
    public bool IsSolid
    {
        get
        {
            return _isSolid;
        }
        set
        {
            _isSolid = value;
        }
    }

    public bool ReverseWireTriangles
    {
        get
        {
            return _reverseWindTriangles;
        }
        set
        {
            _reverseWindTriangles = value;
        }
    }

    public bool UseGravity 
    {
        get
        {
            return _useGravity;
        }
        set
        {
            _useGravity = value;
        }
    }

    public bool ShareVertices 
    {
        get
        {
            return _shareVertices;
        }
        set
        {
            _shareVertices = value;
        }
    }

    public bool SmoothVertices 
    {
        get
        {
            return _smoothVertices;
        }
        set
        {
            _smoothVertices = value;
        }
    }

    void OnDrawGizmos()
    {
        return;
        if(meshFilter)
            Gizmos.DrawCube(meshFilter.mesh.bounds.center + transform.position, 
            new Vector3(meshFilter.mesh.bounds.size.x * transform.localScale.x, 
                        meshFilter.mesh.bounds.size.y * transform.localScale.y,
                        meshFilter.mesh.bounds.size.z * transform.localScale.z));
    }

}