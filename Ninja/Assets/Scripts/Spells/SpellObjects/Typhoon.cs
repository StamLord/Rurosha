using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Typhoon : SpellObject
{
    [Header("References")]
    [SerializeField] private ParticleSystem vfx;

    [Header("Settings")]
    [SerializeField] private float effectEnterRadius;
    [SerializeField] private float effectEndRadius;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float duration;

    [Header("Orbit Velocity Settings")]
    [SerializeField] private float perpendicularForce;
    [SerializeField] private float radius;
    [SerializeField] private float centripetalForce;
    [SerializeField] private float liftForce;

    [Header("Orbit Height Settings")]
    [SerializeField] private float maxRandomHeight = 1f;

    [Header("Realtime Data")]
    [SerializeField] List<Rigidbody> rigidbodies = new List<Rigidbody>();
    [SerializeField] List<float> randomHeights = new List<float>();
    [SerializeField] bool active;
    [SerializeField] float startTime;
    
    public override void Activate(SpellManager spellManager)
    {
        active = true;
        startTime = Time.time;
        vfx.Play();
    }

    private void Deactivate()
    {
        active = false;
        vfx.Stop();
        rigidbodies.Clear();
    }

    private void Update()
    {
        if(active == false)
            return;
            
        AddRigidbodies();
        UpdateVelocities();
        RemoveRigidbodies();

        if(Time.time - startTime > duration)
            Deactivate();
    }

    private void AddRigidbodies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, effectEnterRadius, layerMask);
        foreach (Collider c in colliders)
        {
            Rigidbody r = c.GetComponent<Rigidbody>();
            if(r == null || rigidbodies.Contains(r)) continue;
            rigidbodies.Add(r);
            if(rigidbodies.Count > randomHeights.Count) 
                randomHeights.Add(Random.Range(0f, maxRandomHeight));
        }
    }

    private void RemoveRigidbodies()
    {
        foreach (Rigidbody r in rigidbodies)
        {
            if(Vector3.Distance(r.position, transform.position) > effectEndRadius)
                rigidbodies.Remove(r);
        }
    }
    
    private void UpdateVelocities()
    {
        int index = 0;
        foreach(Rigidbody r in rigidbodies)
        {
            Vector3 dir = r.position - transform.position;
            dir.Normalize();

            Vector3 flatDir = dir;
            flatDir.y = 0;
            flatDir.Normalize();

            Vector3 perpendicular = Vector3.Cross(transform.up, dir).normalized * perpendicularForce;
            Vector3 centripetal = ((transform.position + flatDir * radius + transform.up * randomHeights[index]) - r.position).normalized * centripetalForce;
            Vector3 upwards = Vector3.up * liftForce;

            Debug.DrawRay(r.position, perpendicular, Color.red);
            Debug.DrawRay(r.position, centripetal, Color.blue);
            Debug.DrawRay(r.position, upwards, Color.yellow);

            r.velocity = perpendicular + centripetal + upwards;

            index++;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, effectEnterRadius);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, effectEndRadius);
    }
}
