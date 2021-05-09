using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemAI : MonoBehaviour
{
    private Vector3 startingPosition;

    Vector3 GetRandomDir()
    {
        return new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f));
    }

    void Start()
    {
            startingPosition = transform.position;
    }

    Vector3 GetRoamingPosition()
    {
        return startingPosition + GetRandomDir() * Random.Range(10f, 70f);
    }

    void Update()
    {
        
    }
}
