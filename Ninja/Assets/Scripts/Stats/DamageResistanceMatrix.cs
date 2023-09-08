using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DamageResistanceMatrix : ResistanceMatrix
{
    [SerializeField] private ElementalResistance blunt = new ElementalResistance();
    [SerializeField] private ElementalResistance slash = new ElementalResistance();
    [SerializeField] private ElementalResistance pierce = new ElementalResistance();

    private Dictionary<Modifier, ElementalResistance> modifiers = new Dictionary<Modifier, ElementalResistance>();
    
    public Resistance GetResistance(DamageType damageType)
    {
        ElementalResistance damageResistance = FindDamageTypeResistance(damageType);
        
        // Default is 0%
        if(damageResistance == null) return Resistance.C;

        return damageResistance.Resistance;
    }

    public Resistance GetUnmodifiedResistance(DamageType damageType)
    {
        ElementalResistance damageResistance = FindDamageTypeResistance(damageType);

        // Default is 0%
        if(damageResistance == null) return Resistance.C; 
        
        return damageResistance.Unmodified;
    }

    private ElementalResistance FindDamageTypeResistance(DamageType damageType)
    {
        switch(damageType)
        {
            case DamageType.Blunt:
                return blunt;
            case DamageType.Slash:
                return slash;
            case DamageType.Pierce:
                return pierce;
        }

        return null;
    }
    public float GetResistanceMult(DamageType damageType)
    {
        return GetResistanceMult(GetResistance(damageType));
    }

    #region Modifiers

    public void AddModifier(DamageTypeResistanceModifier damageModifier)
    {
        ElementalResistance damageResistance = FindDamageTypeResistance(damageModifier.damageType);
        if(damageResistance == null) return;
        
        damageResistance.AddModifier(damageModifier.modifier);
        modifiers.Add(damageModifier.modifier, damageResistance);
    }

    public void AddModifiers(List<DamageTypeResistanceModifier> damageModifiers)
    {
        foreach (var mod in damageModifiers)
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
