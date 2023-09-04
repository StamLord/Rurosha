using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElementalResistanceMatrix
{
    [SerializeField] private ElementalResistance earth;
    [SerializeField] private ElementalResistance water;
    [SerializeField] private ElementalResistance fire;
    [SerializeField] private ElementalResistance wind;
    [SerializeField] private ElementalResistance wood;
    [SerializeField] private ElementalResistance thunder;
    [SerializeField] private ElementalResistance metal;
    [SerializeField] private ElementalResistance ice;

    public ElementalResistance GetResistance(ChakraType element)
    {
        switch(element)
        {
            case ChakraType.EARTH:
                return earth;
            case ChakraType.WATER:
                return water;
            case ChakraType.FIRE:
                return fire;
            case ChakraType.WIND:
                return wind;
            case ChakraType.WOOD:
                return wood;
            case ChakraType.THUNDER:
                return thunder;
            case ChakraType.METAL:
                return metal;
            case ChakraType.ICE:
                return ice;
        }
        
        // Default is 0%
        return ElementalResistance.C;
    }

    public float GetResistanceMult(ChakraType element)
    {
        return GetResistanceMult(GetResistance(element));
    }

    public float GetResistanceMult(ElementalResistance resistance)
    {
        switch(resistance)
        {
            case ElementalResistance.A:
                return 0;
            case ElementalResistance.B:
                return .5f;
            case ElementalResistance.C:
                return 1f;
            case ElementalResistance.D:
                return 1.5f;
            case ElementalResistance.E:
                return 2f;
        }

        return 1f;
    }
}
