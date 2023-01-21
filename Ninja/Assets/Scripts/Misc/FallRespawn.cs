using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallRespawn : MonoBehaviour
{
    [SerializeField] private Rigidbody player;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool heightBased = false;
    [SerializeField] private float minimumWorldY;

    private void FixedUpdate()
    {
        if(heightBased && player.transform.position.y < minimumWorldY)
            Respawn();
    }

    private Vector3 FindClosestSpawn()
    {
        Transform closest = null;
        float distance = Mathf.Infinity;

        foreach(Transform s in spawnPoints)
        {
            float d = Vector3.Distance(player.position, s.position);
            if (d < distance)
            {
                distance = d;
                closest = s;
            }
        }

        return closest? closest.position : spawnPoints[0].position;
    }

    private void Respawn()
    {
        player.velocity = Vector3.zero;
        player.transform.position = FindClosestSpawn();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.root == player.transform.root)
        {
            Respawn();
        }
    }
}
