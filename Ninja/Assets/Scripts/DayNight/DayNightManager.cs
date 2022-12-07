using UnityEngine;
using System;

public class DayNightManager : MonoBehaviour
{
    [Header("Time Display")]
    [SerializeField] private float time;
    [SerializeField] private DayTime dayTime;
    [ReadOnly][SerializeField] private Zodiac zodiacHour;

    [Header("Time Settings")]
    [SerializeField] private float dayInRealHours;
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunLight;
    [SerializeField] private float sunRiseHour;
    [SerializeField] private float sunSetHour;
    [SerializeField] private Vector3 sunRiseRot;
    [SerializeField] private Vector3 sunSetRot;

    [Header("Ambient Color")]
    [SerializeField] private Gradient ambientLight;

    private float timeMultiplier;
    private float dayInRealSeconds;
    private float sunRiseRealSeconds;
    private float sunSetRealSeconds;
    private int zodiacNumber;
    private float zodiacDayPercentage;

    [System.Serializable]
    public struct DayTime
    {
        [ReadOnly] public int hours;
        [ReadOnly] public int minutes;
        [ReadOnly] public int seconds;

        public DayTime(int hours, int minutes, int seconds)
        {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
        }
    }

    public delegate void hourPassedDelegate(DayTime time);
    public delegate void minutePassedDelegate(DayTime time);

    public event hourPassedDelegate OnHourPassed;
    public event minutePassedDelegate OnMinutePassed;

    #region Singleton

    public static DayNightManager instance;

    private void Awake() 
    {
        if(instance != null)
        {
            Debug.LogWarning("More than 1 instance of DayNightManager!");
            Destroy(this);
            return;
        }

        instance = this;
    }

    #endregion

    // Update in editor
    private void OnValidate() 
    {
        CalculateValues();
        UpdateSun();
        UpdateAmbient();
        dayTime = GetDayTime();
        zodiacHour = GetZodiacHour();
    }

    private void CalculateValues()
    {
        timeMultiplier = 24 / dayInRealHours;
        dayInRealSeconds = dayInRealHours * 60 * 60 * timeMultiplier;
        sunRiseRealSeconds = sunRiseHour / 24 * dayInRealSeconds;
        sunSetRealSeconds = sunSetHour / 24 * dayInRealSeconds;

        zodiacNumber =  Enum.GetNames(typeof(Zodiac)).Length;
        zodiacDayPercentage = dayInRealSeconds / zodiacNumber;
    }

    private void Start() 
    {
        CalculateValues();

        DebugCommandDatabase.AddCommand(new DebugCommand(
            "settime", 
            "Sets world time", 
            "settime 18:00:00",
            (string[] parameters) => 
            {
                string[] splitTime = parameters[0].Split(':');
                int hours, minutes, seconds;

                if(splitTime.Length == 3)
                {
                    if(int.TryParse(splitTime[0], out hours))
                        if(int.TryParse(splitTime[1], out minutes))
                            if(int.TryParse(splitTime[2], out seconds))
                            {
                                SetDayTime(hours, minutes, seconds);
                                return "World time was set to: " + parameters[0];
                            }
                }
                else if(splitTime.Length == 2)
                {
                    if(int.TryParse(splitTime[0], out hours))
                        if(int.TryParse(splitTime[1], out minutes))
                        {
                            SetDayTime(hours, minutes, 0);
                            return "World time was set to: " + parameters[0] + ":00";
                        }
                }
                else if(splitTime.Length == 1)
                {
                    if(int.TryParse(splitTime[0], out hours))
                    {
                        SetDayTime(hours, 0, 0);
                        return "World time was set to: " + parameters[0] + ":00:00";
                    }
                }

                return "Invalid time format. Format must be: HH:MM:SS";
            }));

        DebugCommandDatabase.AddCommand(new DebugCommand(
            "gettime", 
            "Gets world time in HH:MM:SS", 
            "gettime",
            (string[] parameters) => 
            {
                DayTime t = GetDayTime();
                return string.Format("{0}:{1}:{2}", t.hours, t.minutes, t.seconds);
            }));
    }

    private void Update()
    {
        ProgressTime();
        UpdateSun();
        UpdateAmbient();

        // Only in editor, update values that can be observed in the inspector
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            zodiacHour = GetZodiacHour();
        }
    }

    private void ProgressTime()
    {
        // Advance time
        time += Time.deltaTime * timeMultiplier;

        // New day
        if(time >= dayInRealSeconds)
            time -= dayInRealSeconds;

        int mm = dayTime.minutes;
        int hh = dayTime.hours;
        dayTime = GetDayTime();

        // Call minute event
        if(dayTime.minutes != mm)
            if(OnMinutePassed != null)
                OnMinutePassed(dayTime);

        // Call hour event
        if(dayTime.hours != hh)
            if(OnHourPassed != null)
                OnHourPassed(dayTime);
    }

    private void UpdateSun()
    {
        if(time < sunRiseRealSeconds)
        {
            sun.transform.rotation = Quaternion.Euler(sunSetRot);
            sun.color = sunLight.Evaluate(0);   
            return;
        }

        if(time > sunSetRealSeconds)
        {
            sun.transform.rotation = Quaternion.Euler(sunRiseRot);   
            sun.color = sunLight.Evaluate(1);
            return;
        }

        // Get percentege between sunrise and sunset
        float p = (time - sunRiseRealSeconds) / (sunSetRealSeconds - sunRiseRealSeconds);
        sun.transform.rotation = Quaternion.Euler(Vector3.Lerp(sunRiseRot, sunSetRot, p));
        sun.color = sunLight.Evaluate(p);
    }

    private void UpdateAmbient()
    {
        if(time < sunRiseRealSeconds || time > sunSetRealSeconds)
        {
            RenderSettings.ambientLight = ambientLight.Evaluate(1);
            return;
        }

        // Get percentege between sunrise and sunset
        float p = (time - sunRiseRealSeconds) / (sunSetRealSeconds - sunRiseRealSeconds);
        RenderSettings.ambientLight = ambientLight.Evaluate(p);
    }

    private float GetDayPercent()
    {
        return time / dayInRealSeconds;
    }

    public float GetTime()
    {
        return time;
    }

    public DayTime GetDayTime()
    {
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time - hours * 3600) / 60);
        int seconds = Mathf.FloorToInt(time - minutes * 60 - hours * 3600);

        return new DayTime(hours, minutes, seconds);
    }

    private void SetDayTime(int hours, int minutes, int seconds)
    {
        float totalTime = hours * 60 * 60;
        totalTime += minutes * 60 ;
        totalTime += seconds;

        time = totalTime;
    }

    public Zodiac GetZodiacHour()
    {
        // Set back time 1 hour (rat starts at 23:00 and not 00:00)
        float t = time - dayInRealSeconds / 24;
        if(t < 0)
            t += dayInRealSeconds;
        
        // Round upwards (23:59 aka time .99 should be rounded up to zodiac 0 aka rat)
        int z = Mathf.CeilToInt(t / zodiacDayPercentage);
        if(z >= zodiacNumber)
            z -= zodiacNumber;

        return (Zodiac)z;
    }
}
