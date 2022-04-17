using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTrail : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private Transform _base;
    
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] private int trailFrameLength;

    [SerializeField] private Vector3 lastTipPos;
    [SerializeField] private Vector3 lastBasePos;

    private Mesh mesh;
    private Vector3[] vertices ;
    private int[] triangles ;
    private int frameCount;
    private int verticesNum = 12;

    private void Start() 
    {
        lastTipPos = _tip.position;    
        lastBasePos = _base.position;

        mesh = new Mesh();
        meshFilter.mesh = mesh;

        vertices = new Vector3[trailFrameLength * verticesNum];
        triangles = new int[vertices.Length];
    }

    private void Update()
    {
        GenerateQuad3();
    }

    private void GenerateQuad2()
    {
         if(frameCount == trailFrameLength * verticesNum)
            frameCount = 0;

            // Quad vertex
            vertices[frameCount] = _tip.position;
            vertices[frameCount+1] = lastTipPos;
            vertices[frameCount+2] = lastBasePos;
            vertices[frameCount+3] = _base.position;

            vertices[frameCount+4] = _tip.position;
            vertices[frameCount+5] = lastTipPos;
            vertices[frameCount+6] = lastBasePos;
            vertices[frameCount+7] = _base.position;

            // Clockwise tris
            triangles[frameCount] = frameCount;
            triangles[frameCount+1] = frameCount+1;
            triangles[frameCount+2] = frameCount+2;

            triangles[frameCount+3] = frameCount;
            triangles[frameCount+4] = frameCount+2;
            triangles[frameCount+5] = frameCount+3;

            // Counter-clock tris
            triangles[frameCount+6] = frameCount+4;
            triangles[frameCount+7] = frameCount+6;
            triangles[frameCount+8] = frameCount+5;

            triangles[frameCount+9] = frameCount+4;
            triangles[frameCount+10] = frameCount+7;
            triangles[frameCount+11] = frameCount+6;

            // Set mesh
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);

            frameCount += verticesNum;

            lastTipPos = _tip.position;
            lastBasePos = _base.position;  
    }

    private void GenerateQuad3()
    {
         if(frameCount == trailFrameLength * verticesNum)
            frameCount = 0;

            // Clockwise Quad
            vertices[frameCount] = _tip.position;
            vertices[frameCount+1] = lastTipPos;
            vertices[frameCount+2] = lastBasePos;

            vertices[frameCount+3] = _tip.position;
            vertices[frameCount+4] = lastBasePos;
            vertices[frameCount+5] = _base.position;

            // Counter-clock Quad
            vertices[frameCount+6] = _tip.position;
            vertices[frameCount+7] = _base.position;
            vertices[frameCount+8] = lastTipPos;

            vertices[frameCount+9] = lastTipPos;
            vertices[frameCount+10] = _base.position;
            vertices[frameCount+11] = lastBasePos;

            // Clockwise tris
            triangles[frameCount] = frameCount;
            triangles[frameCount+1] = frameCount+1;
            triangles[frameCount+2] = frameCount+2;

            triangles[frameCount+3] = frameCount+3;
            triangles[frameCount+4] = frameCount+4;
            triangles[frameCount+5] = frameCount+5;

            // Counter-clock tris
            triangles[frameCount+6] = frameCount+6;
            triangles[frameCount+7] = frameCount+7;
            triangles[frameCount+8] = frameCount+8;

            triangles[frameCount+9] = frameCount+9;
            triangles[frameCount+10] = frameCount+10;
            triangles[frameCount+11] = frameCount+11;

            // Set mesh
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);

            frameCount += verticesNum;

            lastTipPos = _tip.position;
            lastBasePos = _base.position;  
    }

    private void GenerateQuad()
    {
        // Get current mesh data
        Vector3[] vertex = meshFilter.mesh.vertices;
        int[] triangles = meshFilter.mesh.triangles;
        
        // Create new bigger arrays 
        Vector3[] v = new Vector3[vertex.Length + 4];
        int[] t = new int[triangles.Length + 6];

        // Copy data to new arrays
        for(int i = 0; i < vertex.Length; i++)
            v[i] = vertex[i];
        for(int j = 0; j < triangles.Length; j++)
            t[j] = triangles[j];

        // Add new data
        v[vertex.Length] = _tip.position;
        v[vertex.Length+1] = lastTipPos;
        v[vertex.Length+2] = lastBasePos;
        v[vertex.Length+3] = _base.position;

        t[triangles.Length] = vertex.Length;
        t[triangles.Length+1] = vertex.Length+1;
        t[triangles.Length+2] = vertex.Length+2;
        t[triangles.Length+3] = vertex.Length;
        t[triangles.Length+4] = vertex.Length+1;
        t[triangles.Length+5] = vertex.Length+3;

        // Set new mesh data
        meshFilter.mesh.SetVertices(v);
        meshFilter.mesh.SetTriangles(t, 0);

        Debug.DrawRay(_tip.position, lastTipPos - _tip.position, Color.yellow, 1f);
        Debug.DrawRay(_base.position, lastBasePos - _base.position, Color.yellow, 1f);
    }
}