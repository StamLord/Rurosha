using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallRespawn : MonoBehaviour
{
    [SerializeField] private Rigidbody player;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minimumWorldY;

    void FixedUpdate()
    {
        if(player.transform.position.y < minimumWorldY)
            Respawn();
    }

    private void Respawn()
    {
        player.velocity = Vector3.zero;
        player.transform.position = spawnPoint.position;
    }
}
