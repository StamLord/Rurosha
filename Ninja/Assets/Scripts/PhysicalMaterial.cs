using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType {Stone, Wood, Flesh}

public class PhysicalMaterial : MonoBehaviour
{
    [SerializeField] private MaterialType mType;
    [SerializeField] private GameObject smallBloodPrefab;
    [SerializeField] private GameObject BigBloodPrefab;
    
    public void CollideEffect(Vector3 position)
    {
        switch(mType)
        {
            case MaterialType.Stone:
                Debug.Log("Spark");
                break;
            case MaterialType.Wood:
                Debug.Log("Splinter");
                break;
            case MaterialType.Flesh:
                Debug.Log("Blood");
                if(smallBloodPrefab)
                {
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0f,1f) * 360, 0);
                    GameObject go = Instantiate(smallBloodPrefab, position, rotation);
                }
                break;
        }
    }
}
