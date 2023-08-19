using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeatConductor : MonoBehaviour, IHeatConductor
{
    [SerializeField] private float temperature;
    [SerializeField] private float conductivity;

    [SerializeField] private float catchFireTemperature;

    public float Temperature {get{return temperature;} private set {temperature = value;}}
    public float Conductivity {get{return conductivity;}}

    [SerializeField] private HeatConductor delegateHeatConductor;
    
    [SerializeField] private List<HeatEvent> events = new List<HeatEvent>();

    [System.Serializable]
    public class HeatEvent
    {
        public float temperature;
        public UnityEvent action;
        public bool played;
    }

    public void Conduct(float sourceTemperature)
    {
        if(delegateHeatConductor)
            delegateHeatConductor.Conduct(sourceTemperature);
        else
            Temperature += conductivity * Mathf.Max(0, sourceTemperature - temperature) * Time.deltaTime;
    }

    private void Update()
    {
        Diffuse();
        for(int i = 0; i < events.Count; i++)
        {
            if(Temperature >= events[i].temperature && events[i].played == false)
            {    
                if(events[i].action != null) 
                {
                    events[i].action.Invoke();
                    events[i].played = true;
                }
            }
        }
    }

    protected void Diffuse()
    {
        Temperature -= conductivity * temperature *.1f * Time.deltaTime;
    }
}
