using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolData", menuName = "Pools/PoolData")]
public class PoolData : ScriptableObject
{
    [SerializeField] private int startAmount = 50;
    [SerializeField] private GameObject prefab;

    private Pool pool;
    public Pool Pool {
        get {
                if(pool == null)
                    pool = new Pool(prefab, startAmount);
                return pool;
            }}
}
