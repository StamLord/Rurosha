using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeatConductor : MonoBehaviour
{
    [SerializeField] private float temperature;
    [SerializeField] private float conductivity;

    [SerializeField] private float catchFireTemperature;

    public float Temperature {get{return temperature;} private set {temperature = value;}}
    public float Conductivity {get{return conductivity;}}

    [SerializeField] private HeatConductor delegateHeatConductor;
    
    [SerializeField] private List<HeatEvent> events = new List<HeatEvent>();

    [System.Serializable]
    public struct HeatEvent
    {
        public float temperature;
        public UnityEvent action;
    }

    public void Conduct(float sourceTemperature)
    {
        if(delegateHeatConductor)
            delegateHeatConductor.Conduct(sourceTemperature);
        else
            Temperature += conductivity * Mathf.Max(0, sourceTemperature - temperature) * Time.deltaTime;
    }

    void Update()
    {
        Diffuse();
        foreach(HeatEvent e in events)
        {
            if(e.temperature >= Temperature)
                if(e.action != null) e.action.Invoke();
        }
    }

    protected void Diffuse()
    {
        Temperature -= conductivity * temperature *.1f * Time.deltaTime;
    }
}
