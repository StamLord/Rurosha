using UnityEngine;

[System.Serializable]
public class ElementalResistanceMatrix : ResistanceMatrix
{
    [SerializeField] private ElementalResistance earth = ElementalResistance.C;
    [SerializeField] private ElementalResistance water = ElementalResistance.C;
    [SerializeField] private ElementalResistance fire = ElementalResistance.C;
    [SerializeField] private ElementalResistance wind = ElementalResistance.C;
    [SerializeField] private ElementalResistance wood = ElementalResistance.C;
    [SerializeField] private ElementalResistance thunder = ElementalResistance.C;
    [SerializeField] private ElementalResistance metal = ElementalResistance.C;
    [SerializeField] private ElementalResistance ice = ElementalResistance.C;

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
}
