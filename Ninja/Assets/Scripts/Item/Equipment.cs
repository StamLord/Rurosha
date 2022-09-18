using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Items/Equipment", order = 3)]
public class Equipment : Item
{
    public EquipmentType equipmentType;
    public EquipmentLayer equipmentLayer;

    public int visualIndex;
    
    public int bluntDefense;
    public int slashDefense;
    public int pierceDefense;

    [System.Serializable]
    public struct Palette 
    {
        public Color primary;
        public Color secondary;
    }

    public Texture[] patterns;
    public Palette[] palettes;

    public int palette;
    public int pattern;

    public override void Randomize()
    {
        base.Randomize();
        palette = GetRandomPalette();
        pattern = GetRandomPattern();
    }

    private int GetRandomPalette()
    {
        return Random.Range(0, palettes.Length);
    }

    private int GetRandomPattern()
    {
        return Random.Range(0, patterns.Length);
    }

    public Palette GetPalette()
    {
        if(palette > palettes.Length || palette == 0)
            return new Palette();
        return palettes[palette];
    }

    public Texture GetPattern()
    {
        if(pattern > patterns.Length || pattern == 0)
            return null;
        return patterns[pattern];
    }
}

