using UnityEngine;

[System.Serializable]
public struct Modifier
{
    public enum ModifierType {FLAT, PERCENTAGE, SET}

    [SerializeField] private ModifierType modfierType;
    [SerializeField] private float value;
    [SerializeField] private bool isTemporary;
    [SerializeField] private float duration;
    [SerializeField] private bool isTimeSensitive;
    [SerializeField] private int startHour;
    [SerializeField] private int endHour;

    public bool IsTemporary { get {return isTemporary; }}
    public float Duration { get {return duration; }}
    public bool IsTimeSensitive { get {return isTimeSensitive; }}
    public int StartHour { get {return startHour; }}
    public int EndHour { get {return endHour; }}

    private bool IsActiveTime(DayNightManager.DayTime dayTime)
    {
        if(isTimeSensitive)
        {
            bool active = false;

            // For example: Between 06:00 to 18:00 
            if(startHour < endHour)
                active = dayTime.hours >= startHour && dayTime.hours < endHour;
            // For example: Between 23:00 to 1:00 
            else if (endHour < startHour) 
                active = (dayTime.hours >= startHour || dayTime.hours < endHour);
            
            return active;
        }

        // If not time sensitive, modifier is always active
        return true;
    }

    public float Process(float initial, DayNightManager.DayTime dayTime)
    {
        // Not within active time
        if(IsActiveTime(dayTime) == false) return initial;

        float newValue = initial;

        // Modification
        switch(modfierType)
        {
            case ModifierType.FLAT:
                newValue = initial + value;
                break;
            case ModifierType.PERCENTAGE:
                newValue = initial * value;
                break;
            case ModifierType.SET:
                newValue = value;
                break;
        }

        return newValue;
    }
}
