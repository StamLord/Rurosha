using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform follow;
    [SerializeField] private ParticleSystem[] weathers;
    [SerializeField] private DayNightManager dayNightManager;

    [Header("Settings")]
    [SerializeField] private float weatherCycle = 3600;
    [SerializeField] private int weatherOnStart = 0;

    [Header("Real Time Data")]
    [SerializeField] private Vector3 wind;
    [SerializeField] private ParticleSystem active;
    [SerializeField] private float lastChange;
    
    private Vector3 offset;

    private void OnValidate() 
    {
        UpdateWeatherWind();
    }

    private void Start() 
    {
        SetWeather(0);
        offset = transform.position - follow.position;
    }

    private void Update() 
    {
        // Switch weather every cycle
        if(dayNightManager.GetTime() - lastChange > weatherCycle)    
            SetWeather(Random.Range(0, weathers.Length));

        // Move weather with player
        transform.position = follow.position + offset;
    }

    public void SetWeather(int index)
    {
        if(index >= weathers.Length)
        {
            ClearWeather();
            return;
        }
        
        if(active) 
            active.Stop();
        
        weathers[index].Play();
        active = weathers[index];

        lastChange = dayNightManager.GetTime();
    }

    public void ClearWeather()
    {
        if(active)
            active.Stop();
        
        lastChange = dayNightManager.GetTime();
    }

    public void SetWind(Vector3 wind)
    {
        this.wind = wind;
        UpdateWeatherWind();
    }

    private void UpdateWeatherWind()
    {
        if(active == false)
            return;

        var v = active.velocityOverLifetime;
        v.enabled = true;
        v.xMultiplier = wind.x;
        v.yMultiplier = wind.y;
        v.zMultiplier = wind.z;
    }
}
