using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTree : MonoBehaviour
{
    [Header("Branch")]
    [SerializeField] private GameObject branchPrefab;

    [Header("Emission")]
    [SerializeField] private int seed;
    [SerializeField] private int number;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;

    [Header("Rotation")]
    [SerializeField] private float rotationXValue;
    [SerializeField] private float rotationXRandom;

    [SerializeField] private float rotationYValue;
    [SerializeField] private float rotationYRandom;

    [SerializeField] private float rotationZValue;
    [SerializeField] private float rotationZRandom;

    [Header("Scale")]
    [SerializeField] private float scaleValue;
    [SerializeField] private float scaleRandom;
    [SerializeField] private AnimationCurve scaleFalloff;

    [SerializeField] private List<GameObject> instanced = new List<GameObject>();

    private void OnValidate() 
    {
        UpdateTree();    
    }

    void UpdateTree()
    {
        if(branchPrefab == null)    
            return;

        Random.InitState(seed);
        
        //ClearTree();

        int delta = Mathf.Max(0, number - instanced.Count);
        for (int i = 0; i < delta; i++)
        {
            GameObject b = Instantiate(branchPrefab, transform);
            instanced.Add(b);
        }

        
        for (int i = 0; i < instanced.Count; i++)
        {
            if(i > number)
                instanced[i].SetActive(false);
            else
            {
                instanced[i].SetActive(true);
                float randHeight = Random.Range(minHeight, maxHeight);
                Vector3 randPos = new Vector3(0, transform.localScale.y * randHeight, 0);
                
                Vector3 randScale = Vector3.one * (scaleValue + scaleRandom * Random.Range(.2f, 2f));
                float heightPercentage = (randHeight - minHeight) / (maxHeight - minHeight);
                randScale *= scaleFalloff.Evaluate(heightPercentage);

                Quaternion randRot = Quaternion.Euler(
                    new Vector3(
                        rotationXValue + rotationXRandom * Random.Range(0, 360),
                        rotationYValue + rotationYRandom * Random.Range(0, 360),
                        rotationZValue + rotationZRandom * Random.Range(0, 360)));
        
                instanced[i].transform.localPosition = randPos;
                instanced[i].transform.localScale = randScale;
                instanced[i].transform.localRotation = randRot;
            }
        }
    }

    private void ClearTree()
    {
        foreach (GameObject branch in instanced)
            Destroy(branch);

        instanced.Clear();
    }
}
