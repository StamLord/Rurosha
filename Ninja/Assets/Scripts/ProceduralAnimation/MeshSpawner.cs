using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSpawner : MonoBehaviour
{
    [SerializeField] private bool isEmitting;
    [SerializeField] private Transform transform;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    [SerializeField] private float amountPerSecond = 5;
    [SerializeField] private float amountPerDistance = 5;
    [SerializeField] private float amountPerRotation = 5;

    [SerializeField] private float lifeTime = 5;

    private bool lastIsEmitting;

    private float perSecond;
    private float lastSpawn;
    private float perDistance;
    private Vector3 lastPosition;
    private float totalDistance;

    private float perRotation;
    private Quaternion lastRotation;
    private float totalRotation;

    [System.Serializable]
    public struct MeshPos
    {
        public Vector3 position;
        public Quaternion rotation;
        public float birthTime;
    }

    [SerializeField] private List<MeshPos> meshes = new List<MeshPos>();

    void Start()
    {
        perSecond = 1 / amountPerSecond;
        perDistance = 1 / amountPerDistance;
        perRotation = 1 / amountPerRotation;

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        // Remove meshes
        foreach(MeshPos mp in meshes)
        {
            if(mp.birthTime < Time.time - lifeTime)
                meshes.Remove(mp);
            else // Since they are added in order, no need to continue checking after finding one that is not expired yet
                break;
        }
        
        // Do nothing if not emitting
        if(isEmitting == false)
            return;
        
        float posDist = Vector3.Distance(transform.position, lastPosition);
        float rotDist = Vector3.Distance(transform.eulerAngles, lastRotation.eulerAngles);

        // Don't draw if delta is too small
        if(posDist < 0.1f && rotDist < 0.1f)
            return;
        
        // Frame Spawn
        if(amountPerDistance != 0)
        {
            AddMesh(transform.position, transform.rotation);

            if(meshes.Count >= amountPerDistance)
                meshes.RemoveAt(0);
        }

        // // Time spawn
        // if(Time.time - lastSpawn > perSecond)
        // {
        //     lastSpawn = Time.time;

        //     MeshPos mp = new MeshPos();
        //     mp.birthTime = Time.time;
        //     mp.position = transform.position;
        //     mp.rotation = transform.rotation;

        //     meshes.Add(mp);
        // }

        // Distance spawn
        // if(amountPerDistance != 0)
        // {
        //     float dist = Vector3.Distance(transform.position, lastPosition);
        //     totalDistance += dist;
        //     int toSpawn = Mathf.FloorToInt(totalDistance * amountPerDistance);
        //     if(toSpawn > 0)
        //     {
        //         IEnumerator co = AddMultipleMeshes(lastPosition, transform.position, lastRotation, transform.rotation, toSpawn);
        //         StartCoroutine(co);
        //         totalDistance -= toSpawn * perDistance;
        //     }
        // }


        // // Rotation Spawn
        // if(amountPerRotation != 0)
        // {
        //     float rotDist = Vector3.Distance(transform.eulerAngles, lastRotation.eulerAngles);
        //     totalRotation += rotDist;
        //     int toSpawn = Mathf.FloorToInt(totalRotation * amountPerRotation);
        //     if(toSpawn > 0)
        //     {
        //         Debug.Log(toSpawn);
        //         Debug.Break();
        //     }
        //     for(int i = 0; i < toSpawn; i++)
        //     {
        //         AddMesh(lastPosition, transform.position, lastRotation, transform.rotation, i / toSpawn);
        //     }

        //     totalRotation -= toSpawn * perRotation;
        // }
        
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastIsEmitting = isEmitting;
    }

    private void LateUpdate() 
    {
        /// Draw all meshes for 1 frame
        DrawMeshes();
    }

    private void DrawMeshes()
    {
        foreach(MeshPos mp in meshes)
            Graphics.DrawMesh(mesh, mp.position, mp.rotation, material, 0);
    }

    private void AddMesh(Vector3 pos1, Vector3 pos2, Quaternion rot1, Quaternion rot2, float percent)
    {
        MeshPos mp = new MeshPos();
        mp.birthTime = Time.time;
        
        mp.position = Vector3.Lerp(pos1, pos2, percent);
        mp.rotation = Quaternion.Lerp(rot1, rot2, percent);

        meshes.Add(mp);
    }

    private void AddMesh(Vector3 pos, Quaternion rot)
    {
        MeshPos mp = new MeshPos();
        mp.birthTime = Time.time;
        
        mp.position = pos;
        mp.rotation = rot;

        meshes.Add(mp);
    }

    private IEnumerator AddMultipleMeshes(Vector3 pos1, Vector3 pos2, Quaternion rot1, Quaternion rot2, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Debug.Log(i);
            AddMesh(lastPosition, transform.position, lastRotation, transform.rotation, i / amount);
            yield return null;
        }
    }
}
