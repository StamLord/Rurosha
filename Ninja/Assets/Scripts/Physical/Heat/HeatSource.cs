using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSource : MonoBehaviour
{
    [SerializeField] private float _temperature;
    [SerializeField] private float _radius;
    [SerializeField] private AnimationCurve _falloff;

    [SerializeField] private bool debug;

    void Update()
    {
        HeatDiffuse();
    }

    void HeatDiffuse()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        
        foreach(Collider col in colliders)
        {
            HeatConductor hc = col.GetComponent<HeatConductor>();
            
            if(hc)
            {
                float falloffSample = _falloff.Evaluate(Vector3.Distance(transform.position, hc.transform.position) / _radius);
                hc.Conduct(_temperature * falloffSample);
            }
        }
    }

    private void OnDrawGizmos() 
    {
        if(debug == false) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
