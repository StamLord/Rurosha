using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEquipmentManager : MonoBehaviour
{
    [SerializeField] private Texture[] patterns;
    [SerializeField] private Color[] colors;

    public static RandomEquipmentManager instance;

    private void Awake() 
    {
        if(instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("More than 1 RandomEquipmentManager instance. Destroying " + gameObject.name);
            Destroy(this);
        }
    }

    public Color GetColor(int index)
    {
        if(index > colors.Length)
            return Color.black;
        return colors[index];
    }

    public Texture GetPattern(int index)
    {
        if(index > patterns.Length)
            return null;
        return patterns[index];
    }

    public int GetRandomColor()
    {
        return Random.Range(0, colors.Length);
    }

    public int GetRandomPattern()
    {
        return Random.Range(0, patterns.Length);
    }
}
