using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHeatSource : HeatSource
{   
    private void OnParticleCollision(GameObject other)
    {
        IHeatConductor hc = other.GetComponentInChildren<IHeatConductor>();
        
        if(debug) Debug.Log("ParticleHeatSource collision with: " + other.name + " IHeatConductor: " + hc);

        if(hc == null) return;

        Diffuse(hc);
    }

    private void Diffuse(IHeatConductor conductor)
    {
        Transform colTransform = ((Component)conductor).transform;
        conductor.Conduct(_temperature);
    }
}
