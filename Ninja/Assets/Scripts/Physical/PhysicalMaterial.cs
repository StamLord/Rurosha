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

    private Camera cam;
    
    public void CollideEffect(Vector3 position, int damage)
    {
        CollideEffect(position, damage, DamageType.Blunt);
    }

    public void CollideEffect(Vector3 position, int damage, DamageType damageType = DamageType.Blunt)
    {
        if(cam == null)
            cam = Camera.main;

        Vector3 dir = cam.transform.position - position;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        
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
        Instantiate(vfx, position, rotation, transform); // Change to pool
    }
}
