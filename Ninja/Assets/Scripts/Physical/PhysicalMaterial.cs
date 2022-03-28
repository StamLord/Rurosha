using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType {Stone, Wood, Flesh}

public class PhysicalMaterial : MonoBehaviour
{
    [SerializeField] private MaterialType mType;
    [SerializeField] private GameObject rockBluntPrefab;
    [SerializeField] private GameObject rockSharpPrefab;

    [SerializeField] private GameObject woodBluntPrefab;
    [SerializeField] private GameObject woodSharpPrefab;

    [SerializeField] private GameObject smallBloodPrefab;
    [SerializeField] private GameObject bigBloodPrefab;
    
    public void CollideEffect(Vector3 position, int damage = -1)
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
                if(damage > 10 && bigBloodPrefab)
                {
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0f,1f) * 360, 0);
                    GameObject go = Instantiate(bigBloodPrefab, position, rotation);
                }
                else if(smallBloodPrefab)
                {
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0f,1f) * 360, 0);
                    GameObject go = Instantiate(smallBloodPrefab, position, rotation);
                }
                break;
        }
    }
}
