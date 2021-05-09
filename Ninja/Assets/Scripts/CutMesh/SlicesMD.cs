using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicesMD 
{
    private Mesh _originalMesh;
    private Plane _plane;

    private List<Vector3> pVertices = new List<Vector3>();
    private List<int> pTriangles = new List<int>();
    private List<Vector3> pNormals = new List<Vector3>();
    private List<Vector2> pUvs = new List<Vector2>();
    public Mesh PositiveSideMesh {get; private set;}

    private List<Vector3> nVertices = new List<Vector3>();
    private List<int> nTriangles = new List<int>();
    private List<Vector3> nNormals = new List<Vector3>();
    private List<Vector2> nUvs = new List<Vector2>();
    public Mesh NegativeSideMesh {get; private set;}

    private List<Vector3> verticesAlongPlane = new List<Vector3>();
    
    private List<Vector3> pSliceVertices = new List<Vector3>();
    private List<int> pSliceTriangles = new List<int>();
    private List<Vector3> pSliceNormals = new List<Vector3>();
    private List<Vector2> pSliceUvs = new List<Vector2>();

    private List<Vector3> nSliceVertices = new List<Vector3>();
    private List<int> nSliceTriangles = new List<int>();
    private List<Vector3> nSliceNormals = new List<Vector3>();
    private List<Vector2> nSliceUvs = new List<Vector2>();


    public SlicesMD(Mesh mesh, Plane plane, bool isSolid, Material insideMat = null)
    {
        _plane = plane;
        _originalMesh = mesh;

        ComputeNewMeshes(_originalMesh, _plane, insideMat);
    }

    void ComputeNewMeshes(Mesh mesh, Plane plane, Material insideMat)
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
            uv = uvs[triangles[i]],
            side = plane.GetSide(vertices[triangles[i]])
            };

            vertexMetadata v2 = new vertexMetadata {
            position = vertices[triangles[i+1]],
            normal = normals[triangles[i+1]],
            uv = uvs[triangles[i+1]],
            side = plane.GetSide(vertices[triangles[i+1]])
            };

            vertexMetadata v3 = new vertexMetadata {
            position = vertices[triangles[i+2]],
            normal = normals[triangles[i+2]],
            uv = uvs[triangles[i+2]],
            side = plane.GetSide(vertices[triangles[i+2]])
            };

            // Check if all on the same side of the plane
            if(v1.side == v2.side && v2.side == v3.side)
            {
                //Add triangle to relevant side
                triangleMetadata triangleMeta = new triangleMetadata {
                    v1Meta = v1,
                    v2Meta = v2,
                    v3Meta = v3
                };

                AddTriangle(triangleMeta ,v1.side);
            }
            // We need to find the intersections and create new triangles
            else
            {
                vertexMetadata intersection1 = new vertexMetadata();
                vertexMetadata intersection2 = new vertexMetadata();

                triangleMetadata newTriangle1 = new triangleMetadata();
                triangleMetadata newTriangle2 = new triangleMetadata();
                triangleMetadata newTriangle3 = new triangleMetadata();

                if(v2.side == v3.side) // v1 is alone
                {
                    intersection1 = GetIntersectionVertex(v2, v1, plane);
                    intersection2 = GetIntersectionVertex(v3, v1, plane);

                    newTriangle1.v1Meta = intersection1;
                    newTriangle1.v2Meta = v2;
                    newTriangle1.v3Meta = v3;
                    AddTriangle(newTriangle1, v2.side);

                    newTriangle2.v1Meta = intersection1;
                    newTriangle2.v2Meta = v3;
                    newTriangle2.v3Meta = intersection2;
                    AddTriangle(newTriangle2, v2.side);

                    newTriangle3.v1Meta = v1;
                    newTriangle3.v2Meta = intersection1;
                    newTriangle3.v3Meta = intersection2;
                    AddTriangle(newTriangle3, !v2.side);
                }
                else if (v1.side == v3.side) // v2 is alone
                {
                    intersection1 = GetIntersectionVertex(v1, v2, plane);
                    intersection2 = GetIntersectionVertex(v3, v2, plane);

                    newTriangle1.v1Meta = intersection1;
                    newTriangle1.v2Meta = v3;
                    newTriangle1.v3Meta = v1;
                    AddTriangle(newTriangle1, v1.side);

                    newTriangle2.v1Meta = intersection1;
                    newTriangle2.v2Meta = intersection2;
                    newTriangle2.v3Meta = v3;
                    AddTriangle(newTriangle2, v1.side);

                    newTriangle3.v1Meta = intersection1;
                    newTriangle3.v2Meta = v2;
                    newTriangle3.v3Meta = intersection2;
                    AddTriangle(newTriangle3, !v1.side);
                }
                else // v3 is alone
                {
                    intersection1 = GetIntersectionVertex(v1, v3, plane);
                    intersection2 = GetIntersectionVertex(v2, v3, plane);

                    newTriangle1.v1Meta = v1;
                    newTriangle1.v2Meta = v2;
                    newTriangle1.v3Meta = intersection1;
                    AddTriangle(newTriangle1, v1.side);

                    newTriangle2.v1Meta = v2;
                    newTriangle2.v2Meta = intersection2;
                    newTriangle2.v3Meta = intersection1;
                    AddTriangle(newTriangle2, v1.side);

                    newTriangle3.v1Meta = intersection1;
                    newTriangle3.v2Meta = intersection2;
                    newTriangle3.v3Meta = v3;
                    AddTriangle(newTriangle3, !v1.side);
                }

                verticesAlongPlane.Add(intersection1.position);
                verticesAlongPlane.Add(intersection2.position);
            }
        }
        
        FillCrossSection(plane, insideMat);
        SetMeshData();
    }

    void AddTriangle(triangleMetadata triangle, bool isPositiveSide)
    {
        if(isPositiveSide)
            AddTriangle(pVertices, pTriangles, pNormals, pUvs, triangle);
        else
            AddTriangle(nVertices, nTriangles, nNormals, nUvs, triangle);
    }

    void AddTriangle(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uvs, triangleMetadata triangle)
    {
        AddVert(vertices, triangles, normals, uvs, triangle.v1Meta);
        AddVert(vertices, triangles, normals, uvs, triangle.v2Meta);
        AddVert(vertices, triangles, normals, uvs, triangle.v3Meta);
    }

    void AddVert(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uvs, vertexMetadata vMeta)
    {
        triangles.Add(vertices.Count);
        vertices.Add(vMeta.position);
        normals.Add(vMeta.normal);
        uvs.Add(vMeta.uv);
    }

    vertexMetadata GetIntersectionVertex(vertexMetadata v1, vertexMetadata v2, Plane plane)
    {
        Vector3 direction = v2.position - v1.position;
        float distance;
        plane.Raycast(new Ray(v1.position, direction), out distance);
        float normalizedDistance = distance / direction.magnitude;
        
        return new vertexMetadata
        {
            position = Vector3.Lerp(v1.position, v2.position, normalizedDistance),
            normal = Vector3.Lerp(v1.normal, v2.normal, normalizedDistance),
            uv = Vector2.Lerp(v1.uv, v2.uv, normalizedDistance)
        };
    }

    void FillCrossSection(Plane plane, Material material)
    {
        Vector3 middle = GetHalfwayPoint(verticesAlongPlane);

        vertexMetadata middleVertex = new vertexMetadata
        {
            position = middle,
            uv = new Vector2(.5f, .5f)
        };

        Vector3 midUV = middleVertex.uv;
        
        for(int i = 0; i < verticesAlongPlane.Count; i += 2)
        {
            vertexMetadata v1 = new vertexMetadata
            {
                position = verticesAlongPlane[i],
                uv = midUV + verticesAlongPlane[i] - middleVertex.position
            };

            vertexMetadata v2 = new vertexMetadata
            {
                position = verticesAlongPlane[i + 1],
                uv = midUV + verticesAlongPlane[i + 1] - middleVertex.position
            };

            triangleMetadata trianglePos, triangleNeg;

            Vector3 triNormal = GetNormal(v1.position, v2.position, middleVertex.position);
            triNormal.Normalize();

            float direction = Vector3.Dot(triNormal, plane.normal);

            if(direction < 0)
            {
                trianglePos.v1Meta = v1;
                trianglePos.v2Meta = v2;
                trianglePos.v3Meta = middleVertex;

                triangleNeg.v1Meta = middleVertex;
                triangleNeg.v2Meta = v2;
                triangleNeg.v3Meta = v1;
            }
            else
            {
                trianglePos.v1Meta = middleVertex;
                trianglePos.v2Meta = v2;
                trianglePos.v3Meta = v1;

                triangleNeg.v1Meta = v1;
                triangleNeg.v2Meta = v2;
                triangleNeg.v3Meta = middleVertex;
            }

            if(material == null)
            {
                AddTriangle(trianglePos, true);
                AddTriangle(triangleNeg, false);
            }
            else
            {
                //AddTriangle(pSliceVertices, pSliceTriangles, pSliceNormals, pSliceUvs, trianglePos);
                //AddTriangle(nSliceVertices, nSliceTriangles, nSliceNormals, nSliceUvs, triangleNeg);
            }
        }
    }

    Vector3 GetNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 side1 = vertex2 - vertex1;
        Vector3 side2 = vertex3 - vertex1;

        return Vector3.Cross(side1, side2);
    }

    Vector3 GetHalfwayPoint(List<Vector3> points)
    {
        Vector3 firstPoint = points[0];
        Vector3 furthestPoint = Vector3.zero;
        float distance = 0;

        foreach (Vector3 point in points)
        {
            float currentDistance = Vector3.Distance(firstPoint, point);
            if(currentDistance > distance)
            {
                distance = currentDistance;
                furthestPoint = point;
            }
        }

        return Vector3.Lerp(firstPoint, furthestPoint, .5f);
    }

    void SetMeshData()
    {
        PositiveSideMesh = new Mesh();
        PositiveSideMesh.vertices = pVertices.ToArray();
        PositiveSideMesh.triangles = pTriangles.ToArray();
        PositiveSideMesh.normals = pNormals.ToArray();
        PositiveSideMesh.uv = pUvs.ToArray();
        PositiveSideMesh.RecalculateNormals();

        NegativeSideMesh = new Mesh();
        NegativeSideMesh.vertices = nVertices.ToArray();
        NegativeSideMesh.triangles = nTriangles.ToArray();
        NegativeSideMesh.normals = nNormals.ToArray();
        NegativeSideMesh.uv = nUvs.ToArray();
        NegativeSideMesh.RecalculateNormals();

        PositiveSideMesh.subMeshCount += 1;
        //PositiveSideMesh.SetTriangles(pSliceTriangles.ToArray() ,PositiveSideMesh.subMeshCount);
    }
}

public struct vertexMetadata
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;
    public bool side;
}

public struct triangleMetadata
{
    public vertexMetadata v1Meta;
    public vertexMetadata v2Meta;
    public vertexMetadata v3Meta;
}
