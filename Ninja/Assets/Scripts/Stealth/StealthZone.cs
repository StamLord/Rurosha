using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float visibilityModifier;
    [SerializeField] private float detectionModifier;
    [SerializeField] private string effectName = "StealthZone";
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private float stopAtParticleTime = .9f;

    [Header("Effected")]
    [SerializeField] private List<StealthAgent> effected = new List<StealthAgent>();

    private void OnTriggerEnter(Collider other) 
    {
        StealthAgent sa = other.transform.root.GetComponentInChildren<StealthAgent>();
        if(sa && effected.Contains(sa) == false)
        {
            effected.Add(sa);
            sa.AddVisibilityModifier(effectName, visibilityModifier);
            sa.AddDetectionModifier(effectName, detectionModifier);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        StealthAgent sa = other.transform.root.GetComponentInChildren<StealthAgent>();
        if(sa)
        {
            effected.Remove(sa);
            sa.RemoveVisibilityModifier(effectName);
            sa.RemoveDetectionModifier(effectName);
        }
    }

    private void Update() 
    {
        if(particleSystem)
        {
            if(particleSystem.time >= stopAtParticleTime)
                Stop();
        }
    }

    public void Stop()
    {
        foreach(StealthAgent sa in effected)
        {
            sa.RemoveVisibilityModifier(effectName);
            sa.RemoveDetectionModifier(effectName);
        }

        effected.Clear();
    }
}
