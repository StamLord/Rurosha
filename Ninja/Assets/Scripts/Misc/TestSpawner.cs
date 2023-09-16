using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TestSpawn
{
    public GameObject spawn;
    public Switch _switch;
    public GameObject spawned;
    
}

public class TestSpawner : MonoBehaviour
{
    [SerializeField] private TestSpawn[] testSpawners;

    private void Start()
    {
        for(int i = 0; i < testSpawners.Length; i++)
        {
            TestSpawn ts = testSpawners[i];
            ts._switch.StateChangeEvent += (bool state) => { 
                if(state)
                    ts.spawned = Instantiate(ts.spawn, Vector3.zero, Quaternion.identity) as GameObject;
                else
                {
                    Destroy(ts.spawned);
                    ts.spawned = null;
                }};
        }
    }
}
