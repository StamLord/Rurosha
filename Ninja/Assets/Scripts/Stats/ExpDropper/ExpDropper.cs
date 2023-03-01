using UnityEngine;

public class ExpDropper : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] private CharacterStats stats;

    [Header("Pool Data")]
    [SerializeField] private PoolData poolData;
    public static Pool pool;

    [Header("Instantiate Position")]
    [SerializeField] private Transform target;
    [SerializeField] private float spawnRadius = 1f;
    [SerializeField] private float heightOffset = 1f;

    private void Start()
    {
        // Initialize pool only if it's null (static member)
        if(pool == null)
            pool = poolData.Pool;
        
        // Subscribe to event
        if(stats)
            stats.OnDeath += DropExp;
    }

    private void DropExp()
    {
        // Drops exp orbs of 10 each
        int amount = stats.ExpDrop / 10;

        for (var i = 0; i < amount; i++)
        {
            GameObject drop = pool.Get();

            Vector3 random = Random.insideUnitSphere * spawnRadius;
            
            drop.transform.position = new Vector3(
                target.position.x + random.x, 
                target.position.y + random.y + heightOffset, 
                target.position.z + random.z);

            Boost boost = drop.GetComponent<Boost>();
            if(boost != null) boost.SetPool(pool);
        }
        
    }
}
