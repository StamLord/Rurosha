using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform follow;
    [SerializeField] private ParticleSystem[] weathers;

    [Header("Settings")]
    [SerializeField] private float weatherCycle = 3600;
    [SerializeField] private int weatherOnStart = 0;
    [SerializeField] private float yOffset;

    [Header("Real Time Data")]
    [SerializeField] private Vector3 wind;
    [SerializeField] private ParticleSystem active;
    [SerializeField] private float lastChange;
    

    private void OnValidate() 
    {
        UpdateWeatherWind();
    }

    private void Start() 
    {
        SetWeather(weatherOnStart);

        DebugCommandDatabase.AddCommand(new DebugCommand(
            "setweather",
            "Sets weather to a specific id",
            "class  <id>",
            (string[] parameters) => {
                int i;
                bool success = int.TryParse(parameters[0], out i);
                if(success)
                    success = SetWeather(i);

                if(success)
                    return "Weather set to id: " + i;
                else
                    return string.Format("Incorrect or out of bounds id. Valid range is [{0}..{1}]", 0, weathers.Length - 1);
            }
        ));

        DebugCommandDatabase.AddCommand(new DebugCommand(
            "clearweather",
            "Clears the weather.",
            "clearweather",
            (string[] parameters) => {
                ClearWeather();
                return "Weather cleared.";
            }
        ));
    }

    private void Update() 
    {
        // Switch weather every cycle
        if(DayNightManager.instance.GetTime() - lastChange > weatherCycle)    
            SetWeather(Random.Range(0, weathers.Length));

        // Move weather with player
        transform.position = follow.position + Vector3.up * yOffset;
    }

    public bool SetWeather(int index)
    {
        if(index < 0 || index >= weathers.Length || weathers[index] == null)
        {
            ClearWeather();
            return false;
        }
        
        if(active) 
            active.Stop();
        
        weathers[index].Play();
        active = weathers[index];

        lastChange = DayNightManager.instance.GetTime();
        return true;
    }

    public void ClearWeather()
    {
        if(active)
            active.Stop();
        
        lastChange = DayNightManager.instance.GetTime();
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
