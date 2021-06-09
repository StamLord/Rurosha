using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatConductor : MonoBehaviour
{
    [SerializeField] private float temperature;
    [SerializeField] private float conductivity;

    [SerializeField] private float catchFireTemperature;

    public float Temperature {get{return temperature;} private set {temperature = value;}}
    public float Conductivity {get{return conductivity;}}

    public void Conduct(float sourceTemperature)
    {
        Temperature += conductivity * Mathf.Max(0, sourceTemperature - temperature) * Time.deltaTime;
    }

    void Update()
    {
        Diffuse();
    }

    protected void Diffuse()
    {
        Temperature -= conductivity * temperature *.1f * Time.deltaTime;
    }
}
