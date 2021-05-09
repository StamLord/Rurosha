using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    public MeshFilter meshFilter;

    public List<Vector3> nVertices = new List<Vector3>();
    public List<int> nTriangles = new List<int>();
    public List<Vector3> nNormals = new List<Vector3>();
    public List<Vector2> nUvs = new List<Vector2>();
    public bool compute;

    void Start()
    {
        if(compute == false)
        {
            nVertices = new List<Vector3>(meshFilter.mesh.vertices);
            nTriangles = new List<int>(meshFilter.mesh.triangles);
            nNormals = new List<Vector3>(meshFilter.mesh.normals);
            nUvs = new List<Vector2>(meshFilter.mesh.uv);
            return;
        }

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = ComputeMesh(meshFilter.mesh);
        meshFilter.mesh.RecalculateNormals();
    }

    Mesh ComputeMesh(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        // Loop through triangles
        for(int i = 0; i < triangles.Length; i += 3)
        {
            vertexMetadata v1 = new vertexMetadata {
            position = vertices[triangles[i]],
            normal = normals[triangles[i]],
            uv = uvs[triangles[i]]           
            };

            vertexMetadata v2 = new vertexMetadata {
            position = vertices[triangles[i+1]],
            normal = normals[triangles[i+1]],
            uv = uvs[triangles[i+1]]
            };

            vertexMetadata v3 = new vertexMetadata {
            position = vertices[triangles[i+2]],
            normal = normals[triangles[i+2]],
            uv = uvs[triangles[i+2]]
            };

            triangleMetadata triangleMeta = new triangleMetadata {
                v1Meta = v1,
                v2Meta = v2,
                v3Meta = v3
            };

            AddTriangle(ref nVertices, ref nTriangles, ref nNormals, ref nUvs, triangleMeta);
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = nVertices.ToArray();
        newMesh.triangles = nTriangles.ToArray();
        newMesh.normals = nNormals.ToArray();
        newMesh.uv = nUvs.ToArray();
        return newMesh;
    }


    void AddTriangle(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, triangleMetadata triangle)
    {
        AddVert(ref vertices, ref triangles, ref normals, ref uvs, triangle.v1Meta);
        AddVert(ref vertices, ref triangles, ref normals, ref uvs, triangle.v2Meta);
        AddVert(ref vertices, ref triangles, ref normals, ref uvs, triangle.v3Meta);
    }

    void AddVert(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, vertexMetadata vMeta)
    {
        triangles.Add(vertices.Count);
        vertices.Add(vMeta.position);
        normals.Add(vMeta.normal);
        uvs.Add(vMeta.uv);
    }

    void Update()
    {
        //meshFilter.mesh.vertices = nVertices.ToArray();
        //meshFilter.mesh.uv = nUvs.ToArray();
    }

    void OnDrawGizmos()
    {
        // for(int i = 0; i < vertices.Length; i++)
        // {
        //     Gizmos.color = Color.white;
        //     Gizmos.DrawRay(transform.position + vertices[i], normals[i]);

        //     Gizmos.color = Color.Lerp(Color.red, Color.green, uvs[i].x);
        //     Gizmos.DrawSphere(vertices[i] + transform.position, .1f);
        // }
        
    }
}
