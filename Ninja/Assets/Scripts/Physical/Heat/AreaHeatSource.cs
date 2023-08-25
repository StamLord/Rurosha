using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaHeatSource : HeatSource
{
    [SerializeField] private float _radius;
    [SerializeField] private AnimationCurve _falloff;

    private void Update()
    {
        HeatDiffuse();
    }

    private void HeatDiffuse()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        
        foreach(Collider col in colliders)
        {
            IHeatConductor hc = col.GetComponent<IHeatConductor>();
            if(hc == null) continue;

            Diffuse(hc);
        }
    }

    private void Diffuse(IHeatConductor conductor)
    {
        Transform colTransform = ((Component)conductor).transform;
        float falloffSample = _falloff.Evaluate(Vector3.Distance(transform.position, colTransform.position) / _radius);
        conductor.Conduct(_temperature * falloffSample);
    }

    private void OnDrawGizmos()
    {
        if(debug == false) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
