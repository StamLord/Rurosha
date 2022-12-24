using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject camera;
    [SerializeField] private float tickRate = 10; // Amount of seconds between ticks
    [SerializeField] private float gracePeriod = 20; // Seconds after encounter without ticks
    public Encounter[] encounters;
    public float[] encounterChance;

    private float lastTick;
    private float lastEncounter;
    private bool inGrace {get { return (Time.time - lastEncounter < gracePeriod);}}

    private void Start()
    {
        // So we don't start in grace period
        lastEncounter = -gracePeriod;
    }

    private void OnTriggerStay(Collider other) 
    {   
        // Ignore any other colliders
        if(other.transform.root != player.transform) return;

        if(inGrace == false && Time.time - lastTick >= tickRate)
            Tick(other.transform.position);
    }

    /// <summary>
    /// Generates a random number (0f-1f) and spawns an encounter that matches the number (per encounterChance)
    /// </summary>
    private void Tick(Vector3 position)
    {
        // Roll odds on enocunters
        float random = Random.Range(0f, 1f);
        
        Debug.Log("Roll: " + random);

        float sum = 0;
        for(int i = 0; i < encounterChance.Length; i++)
        {
            if(random <= sum + encounterChance[i])
            {
                SpawnEncounter(position, encounters[i]);
                break;
            }

            sum += encounterChance[i];
        }

        lastTick = Time.time;
    }

    private void SpawnEncounter(Vector3 position, Encounter encounter)
    {   
        Vector3 spawnOrigin = position;

        // Make sure we spawn behind player
        if(encounter.behindPlayer)
            spawnOrigin -= Vector3.forward * encounter.radius;
        
        // Random point around player
        Vector2 randomPos = Random.insideUnitCircle * encounter.radius;
        Vector3 offset = randomPos;

        // Sample navmesh for a possible location
        NavMeshHit hit;
        if(NavMesh.SamplePosition(position + offset, out hit, 2f, 1))
        {    
            
            // TODO: Use spawn pool
            foreach (GameObject enemy in encounter.enemies)
                Instantiate(enemy, hit.position, Quaternion.identity);
        }

        lastEncounter = Time.time;
    }
}
