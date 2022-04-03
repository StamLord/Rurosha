using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalMaterial : MonoBehaviour
{
    [SerializeField] private MaterialType mType;
    [SerializeField] private GameObject rockBluntPrefab;
    [SerializeField] private GameObject rockSharpPrefab;

    [SerializeField] private GameObject woodBluntPrefab;
    [SerializeField] private GameObject woodSharpPrefab;

    [SerializeField] private GameObject smallBloodPrefab;
    [SerializeField] private GameObject bigBloodPrefab;

    [SerializeField] private GameObject smallSparkPrefab;
    [SerializeField] private GameObject bigSparkPrefab;
    
    public void CollideEffect(Vector3 position, int damage)
    {
        CollideEffect(position, damage, mType);
    }

    public void CollideEffect(Vector3 position, int damage, MaterialType materialType = MaterialType.Stone)
    {
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0f,1f) * 360, 0);
        
        switch(materialType)
        {
            case MaterialType.Stone:
                Debug.Log("Dust");
                break;
            case MaterialType.Wood:
                Debug.Log("Splinter");
                break;
            case MaterialType.Flesh:
                if(damage > 10 && bigBloodPrefab)
                    Instantiate(bigBloodPrefab, position, rotation);
                else if(smallBloodPrefab)
                    Instantiate(smallBloodPrefab, position, rotation);
                break;
            case MaterialType.Metal:
                if(damage > 10 && bigSparkPrefab)
                    Instantiate(bigSparkPrefab, position, rotation);
                else if(smallSparkPrefab)
                    Instantiate(smallSparkPrefab, position, rotation);
                break;
        }
    }
}
