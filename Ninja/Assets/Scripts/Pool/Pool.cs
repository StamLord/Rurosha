using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    private int startAmount;
    private GameObject obj;
    private Vector3 pos = new Vector3(999,999,999);
    private List<GameObject> pool = new List<GameObject>();

    public Pool(GameObject obj, int startAmount = 10)
    {
        this.startAmount = startAmount;
        this.obj = obj;

        // Create initial pool
        int diff = startAmount - pool.Count;
        for (var i = 0; i < diff; i++)
            Return(Create());
    }

    public GameObject Create()
    {
        GameObject o = GameObject.Instantiate(obj, pos, Quaternion.identity);
        return o;
    }

    public GameObject Get()
    {
        if(pool.Count > 0)
        {
            GameObject o = pool[0];
            o.SetActive(true);
            pool.RemoveAt(0);
            return o;
        }
        else
            return Create();
    }

    public void Return(GameObject gameObject)
    {
        gameObject.SetActive(false);
        pool.Add(gameObject);
    }
}
