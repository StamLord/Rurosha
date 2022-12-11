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

    public float Process(float initial)
    {
        float newValue = initial;

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
