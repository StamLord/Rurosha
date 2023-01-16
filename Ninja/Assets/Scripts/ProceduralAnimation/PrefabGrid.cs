using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabGrid : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float diameter;
    [SerializeField] private int resolution;
    [SerializeField] private float randomPosition;
    [SerializeField] private float randomRotation;
    [SerializeField] private int randomSeed;
    [SerializeField] private AnimationCurve heightFalloff;

    [SerializeField] private bool updateObjects;
    [SerializeField] private bool clearObjects;
    [SerializeField] private List<GameObject> instanced = new List<GameObject>();
    [SerializeField] private bool isDebug;

    private void OnValidate() 
    {
        if(clearObjects)
        {
            ClearObjects(instanced);
            clearObjects = false;
        }

        if(updateObjects)
            updateObjects = false;
        
        UpdateObjects();
    }

    private void UpdateObjects()
    {
        if(prefab == null) return;

        StartCoroutine("UpdateObjectsCoroutine");
    }

    private void ClearObjects(List<GameObject> toDelete)
    {
        StartCoroutine("ClearObjectsCoroutine", toDelete);
    }

    IEnumerator UpdateObjectsCoroutine()
    {
        // Init seed
        Random.InitState(randomSeed);

        // Calculate positions
        float increment = diameter / (resolution + 1);
        Vector3 gridOrigin = new Vector3(transform.position.x - increment * resolution * .5f, transform.position.y, transform.position.z - increment * resolution * .5f);

        // Instantiate if needed
        int diff = resolution * resolution - instanced.Count;
        if(diff > 0)
        {
            for (int i = 0; i < diff; i++)
                instanced.Add(Instantiate(prefab, transform.position, Quaternion.identity, transform));
        }
        // Delete if needed
        else if (diff < 0)
        {
            List<GameObject> toDelete = new List<GameObject>();
            for (int i = diff; i < 0; i++)
            {
                toDelete.Add(instanced[0]);
                instanced.RemoveAt(0);

                ClearObjects(toDelete);
            }
        }
        
        // Loop through objects and update positions
        int index = 0;
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                // Position
                Vector2 random = new Vector2(Random.Range(-randomPosition, randomPosition), Random.Range(-randomPosition, randomPosition));
                Vector3 position = gridOrigin + new Vector3(i * increment + random.x, 0, j * increment + random.y);
                Vector3 rotation = new Vector3(0, Random.Range(-randomRotation, randomRotation), 0);

                // Height
                RaycastHit hit;
                if(Physics.Raycast(position + Vector3.up * diameter * .5f, Vector3.down, out hit, diameter))
                    position.y = hit.point.y;

                // Check if too far from center
                Vector3 flatCenter = transform.position;
                flatCenter.y = position.y; // We want a flat distance not taking y axis into account
                float distance = Vector3.Distance(position, flatCenter);
                float yScale = heightFalloff.Evaluate(distance / diameter);
                Vector3 scale = new Vector3(1, yScale, 1);

                // Deactivate objects that are too far
                if(distance> diameter / 2)
                   instanced[index].SetActive(false);
                else
                {
                    instanced[index].SetActive(true);
                    instanced[index].transform.position = position;
                    instanced[index].transform.rotation = Quaternion.Euler(rotation);
                    instanced[index].transform.localScale = scale;
                }
                index += 1;
            }
            yield return null;
        }
    }

    IEnumerator ClearObjectsCoroutine(List<GameObject> toDelete)
    {
        yield return null;

        foreach (var i in toDelete)
        {
            if(i == null)
                continue;
                
            if(Application.isEditor)
            {
                i.SetActive(false);
                DestroyImmediate(i);
            }
            else
                Destroy(i);
        }

        toDelete.Clear();
    }

    private void OnDrawGizmos() 
    {
        if(isDebug == false) 
            return;
        
        // Draw sphere
        Gizmos.DrawWireSphere(transform.position, diameter * .5f);

        // Init seed
        Random.InitState(randomSeed);

        // Draw positions
        float increment = diameter / (resolution + 1);
        Vector3 gridOrigin = new Vector3(transform.position.x - increment * resolution * .5f, transform.position.y, transform.position.z - increment * resolution * .5f);
        Gizmos.DrawCube(gridOrigin, Vector3.one);

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                Vector2 random = new Vector2(Random.Range(-randomPosition, randomPosition), Random.Range(-randomPosition, randomPosition));
                Vector3 position = gridOrigin + new Vector3(i * increment + random.x, 0, j * increment + random.y);

                RaycastHit hit;
                if(Physics.Raycast(position + Vector3.up * diameter * .5f, Vector3.down, out hit, diameter))
                    position.y = hit.point.y;

                // Check if too far from center
                if(Vector3.Distance(position, transform.position) > diameter / 2)
                   continue;

                Gizmos.DrawSphere(position, .1f);
            }
        }
    }
}
