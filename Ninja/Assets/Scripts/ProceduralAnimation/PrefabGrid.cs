using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabGrid : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float radius;
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
            ClearObjects();
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

    private void ClearObjects()
    {
        foreach (var i in instanced)
        {
            if(Application.isEditor)
            {
                if(i)
                {
                    i.SetActive(false);
                    DestroyImmediate(i);
                }
            }
            else
                Destroy(i);
        }

        instanced.Clear();
    }

    IEnumerator UpdateObjectsCoroutine()
    {
        // Init seed
        Random.InitState(randomSeed);

        // Calculate positions
        float increment = radius / (resolution + 1);
        Vector3 gridOrigin = new Vector3(transform.position.x - increment * resolution * .5f, transform.position.y, transform.position.z - increment * resolution * .5f);

        // Instantiate if needed
        int diff = resolution * resolution - instanced.Count;
        if(diff > 0)
        {
            for (int i = 0; i < diff; i++)
                instanced.Add(Instantiate(prefab, transform.position, Quaternion.identity, transform));
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
                if(Physics.Raycast(position + Vector3.up * 5, Vector3.down, out hit, 20f))
                    position.y = hit.point.y;

                // Check if too far from center
                float distance = Vector3.Distance(position, transform.position);
                float yScale = heightFalloff.Evaluate(distance / radius);
                Vector3 scale = new Vector3(yScale, yScale, yScale);

                // Deactivate objects that are too far
                if(distance> radius / 2)
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

    private void OnDrawGizmos() 
    {
        if(isDebug == false) 
            return;
        
        // Draw sphere
        Gizmos.DrawWireSphere(transform.position, radius);

        // Init seed
        Random.InitState(randomSeed);

        // Draw positions
        float increment = radius / (resolution + 1);
        Vector3 gridOrigin = new Vector3(transform.position.x - increment * resolution * .5f, transform.position.y, transform.position.z - increment * resolution * .5f);
        Gizmos.DrawCube(gridOrigin, Vector3.one);

        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                Vector2 random = new Vector2(Random.Range(-randomPosition, randomPosition), Random.Range(-randomPosition, randomPosition));
                Vector3 position = gridOrigin + new Vector3(i * increment + random.x, 0, j * increment + random.y);

                RaycastHit hit;
                if(Physics.Raycast(position + Vector3.up * 5, Vector3.down, out hit, 10f))
                    position.y = hit.point.y;

                // Check if too far from center
                if(Vector3.Distance(position, transform.position) > radius / 2)
                   continue;

                Gizmos.DrawSphere(position, .1f);
            }
        }
    }
}
