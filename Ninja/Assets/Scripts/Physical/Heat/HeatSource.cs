using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSource : MonoBehaviour
{
    [SerializeField] private float _temperature;
    [SerializeField] private float _radius;
    [SerializeField] private AnimationCurve _falloff;

    [SerializeField] public static bool debug;
    
    private void Start()
    {
        DebugCommandDatabase.AddCommand(new DebugCommand(
                "debugheat", 
                "Sets debug of HeatSource to true or false", 
                "debugheat <1/0>", 
                (string[] parameters) => {
                    switch(parameters[0])
                    {
                        case "0":
                            debug = false;
                            return "HeatSource debug set to False";
                        case "1":
                            debug = true;
                            return "HeatSource debug set to True";
                    }
                    return "Parameter should be 1 or 0";
                }));
    }

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

            Transform colTransform = ((Component)hc).transform;
            float falloffSample = _falloff.Evaluate(Vector3.Distance(transform.position, colTransform.position) / _radius);
            hc.Conduct(_temperature * falloffSample);
        }
    }

    private void OnDrawGizmos()
    {
        if(debug == false) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnGUI() 
    {
        if(debug == false) return;

        Camera cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        if(viewportPos.x <= 0 || viewportPos.x >= 1 || viewportPos.y <= 0 || viewportPos.y >= 1 || viewportPos.z < 0) return;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        float y = Screen.height - screenPos.y; // Invert y since camera treats bottom as 0 while Rect treats top as 0
        GUI.Box(new Rect(screenPos.x, y, 100, 50), _temperature.ToString());
    }
}
