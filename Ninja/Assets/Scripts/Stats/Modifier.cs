using UnityEngine;

[System.Serializable]
public struct Modifier
{
    public enum ModfierType {FLAT, PERCENTAGE}

    [SerializeField] private string attribute;
    [SerializeField] private ModfierType modfierType;
    [SerializeField] private float value;

    public string Attribute {get {return attribute;}}

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
