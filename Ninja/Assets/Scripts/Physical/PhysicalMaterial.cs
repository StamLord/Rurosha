using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalMaterial : MonoBehaviour
{
    [SerializeField] private GameObject bluntPrefab;
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private GameObject piercePrefab;

    [SerializeField] private GameObject bigBluntPrefab;
    [SerializeField] private GameObject bigSlashPrefab;
    [SerializeField] private GameObject bigPiercePrefab;

    [SerializeField] private int bigDamageThreshold = 10;
    [SerializeField] private float vfxLifeTime = 10;

    private static Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>();
    
    private Camera cam;
    
    private void Start() 
    {
        // Create pools if none was created for these prefabs
        if(bluntPrefab) CreatePool(bluntPrefab);
        if(slashPrefab) CreatePool(slashPrefab);
        if(piercePrefab) CreatePool(piercePrefab);
        
        if(bigBluntPrefab) CreatePool(bigBluntPrefab);
        if(bigSlashPrefab) CreatePool(bigSlashPrefab);
        if(bigPiercePrefab) CreatePool(bigPiercePrefab);
    }

    private void CreatePool(GameObject prefab)
    {
        // Check if already exists
        if(pools.ContainsKey(prefab)) return;

        pools.Add(prefab, new Pool(prefab));
    }

    public void CollideEffect(Vector3 position, int damage, Vector3 hitUp, DamageType damageType = DamageType.Blunt)
    {
        if(cam == null)
            cam = Camera.main;

        Vector3 dir = (cam.transform.position - position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir, hitUp);

        position += dir * .5f;

        switch(damageType)
        {
            case DamageType.Blunt:
                PlayBigOrSmallVfx(bluntPrefab, bigBluntPrefab, damage, position, rotation);
                break;
            case DamageType.Slash:
                PlayBigOrSmallVfx(slashPrefab, bigSlashPrefab, damage, position, rotation);
                break;
            case DamageType.Pierce:
                PlayBigOrSmallVfx(piercePrefab, bigPiercePrefab, damage, position, rotation);
                break;
        }
    }

    private void PlayBigOrSmallVfx(GameObject smallVfx, GameObject bigVfx, int damage, Vector3 position, Quaternion rotation)
    {
        if(damage > bigDamageThreshold && bigVfx)
            PlayVfx(bigVfx, position, rotation);
        else if(smallVfx)
            PlayVfx(smallVfx, position, rotation);
    }

    private void PlayVfx(GameObject vfx, Vector3 position, Quaternion rotation)
    {
        //Instantiate(vfx, position, rotation, transform); // Change to pool
        if(pools.ContainsKey(vfx))
        {
            GameObject go = pools[vfx].Get();
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.parent = transform;

            StartCoroutine(ReturnVfx(go, pools[vfx], vfxLifeTime));
        }
    }

    private IEnumerator ReturnVfx(GameObject vfx, Pool pool, float time)
    {
        yield return new WaitForSeconds(time);
        pool.Return(vfx);
    }
}
