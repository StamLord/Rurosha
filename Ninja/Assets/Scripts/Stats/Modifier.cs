using UnityEngine;

[System.Serializable]
public struct Modifier
{
    public enum ModfierType {FLAT, PERCENTAGE}

    [SerializeField] private string attribute;
    [SerializeField] private ModfierType modfierType;
    [SerializeField] private float value;
    [SerializeField] private bool isTimeSensitive;
    [SerializeField] private int startHour;
    [SerializeField] private int endHour;

    public string Attribute { get {return attribute; }}
    public bool IsTimeSensitive { get {return isTimeSensitive; }}
    public int StartHour { get {return startHour; }}
    public int EndHour { get {return endHour; }}

    public float Process(float initial, DayNightManager.DayTime dayTime)
    {
        // Time check
        if(isTimeSensitive)
        {
            bool active = false;

            // For example: Between 06:00 to 18:00 
            if(startHour < endHour)
                active = dayTime.hours >= StartHour && dayTime.hours < EndHour;
            // For example: Between 23:00 to 1:00 
            else if (endHour < startHour) 
                active = (dayTime.hours >= startHour || dayTime.hours < endHour);
            Debug.Log("Is active - " + active);
            if(active == false)
                return initial;
        }

        float newValue = initial;

        // Modification
        switch(modfierType)
        {
            case ModfierType.FLAT:
                newValue = initial + value;
                break;
            case ModfierType.PERCENTAGE:
                newValue = initial * value;
                break;
        }

        return newValue;
    }
}
