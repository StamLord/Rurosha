using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class RandomEquipmentManager : MonoBehaviour
{
    [System.Serializable]
    public struct Palette 
    {
        public Color primary;
        public Color secondary;
    }

    [SerializeField] private Texture[] patterns;
    [SerializeField] private Palette[] palettes;

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

    public Palette GetPalette(int index)
    {
        if(index > palettes.Length)
            return new Palette();
        return palettes[index];
    }

    public Texture GetPattern(int index)
    {
        if(index > patterns.Length)
            return null;
        return patterns[index];
    }

    public int GetRandomPalette()
    {
        return Random.Range(0, palettes.Length);
    }

    public int GetRandomPattern()
    {
        return Random.Range(0, patterns.Length);
    }
}
