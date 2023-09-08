using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ElementalResistanceMatrix : ResistanceMatrix
{
    [SerializeField] private ElementalResistance earth = new ElementalResistance();
    [SerializeField] private ElementalResistance water = new ElementalResistance();
    [SerializeField] private ElementalResistance fire = new ElementalResistance();
    [SerializeField] private ElementalResistance wind = new ElementalResistance();
    [SerializeField] private ElementalResistance wood = new ElementalResistance();
    [SerializeField] private ElementalResistance thunder = new ElementalResistance();
    [SerializeField] private ElementalResistance metal = new ElementalResistance();
    [SerializeField] private ElementalResistance ice = new ElementalResistance();

    private Dictionary<Modifier, ElementalResistance> modifiers = new Dictionary<Modifier, ElementalResistance>();
    
    public Resistance GetResistance(ChakraType element)
    {
        ElementalResistance elementalResistance = FindElementalResistance(element);

        // Default is 0%
        if(elementalResistance == null) return Resistance.C; 
        
        return elementalResistance.Resistance;
    }

    public Resistance GetUnmodifiedResistance(ChakraType element)
    {
        ElementalResistance elementalResistance = FindElementalResistance(element);

        // Default is 0%
        if(elementalResistance == null) return Resistance.C; 
        
        return elementalResistance.Unmodified;
    }

    private ElementalResistance FindElementalResistance(ChakraType element)
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

        return null;
    }

    public float GetResistanceMult(ChakraType element)
    {
        return GetResistanceMult(GetResistance(element));
    }

    #region Modifiers

    public void AddModifier(ElementResistanceModifier elementModifier)
    {
        ElementalResistance elementalResistance = FindElementalResistance(elementModifier.element);
        if(elementalResistance == null) return;
        
        elementalResistance.AddModifier(elementModifier.modifier);
        modifiers.Add(elementModifier.modifier, elementalResistance);
    }

    public void AddModifiers(List<ElementResistanceModifier> elementModifiers)
    {
        foreach (var mod in elementModifiers)
            AddModifier(mod);
    }

    public void RemoveModifier(Modifier modifier)
    {
        if(modifiers.ContainsKey(modifier) == false) return;

        modifiers[modifier].RemoveModifier(modifier);
        modifiers.Remove(modifier);
    }

    public void RemoveModifiers(List<Modifier> modifiers)
    {
        foreach (var mod in modifiers)
            RemoveModifier(mod);
    }

    #endregion
}
