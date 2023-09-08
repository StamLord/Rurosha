using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElementalResistance : Modifieable
{
    [SerializeField] private Resistance resistance = Resistance.C;

    public Resistance Resistance { get {return (Resistance)Modified;}}
    public Resistance Unmodified { get {return resistance;}}

    public ElementalResistance(Resistance resistance = Resistance.C)
    {
        this.resistance = resistance;
    }

    public override int CalculateModified()
    {
        if(modifiers.Count < 1) return (int)resistance;

        // Apply all modifiers
        float modValue = (float)resistance;
        DayNightManager.DayTime dayTime = DayNightManager.instance.GetDayTime();

        for (int i = 0; i < modifiers.Count; i++)
            modValue = modifiers[i].Process(modValue, dayTime);
        
        // Round to whole number
        switch(roundType)
        {
            case RoundType.DOWN:
                return Mathf.FloorToInt(modValue);
            case RoundType.UP:
                return Mathf.CeilToInt(modValue);
        }

        return Mathf.RoundToInt(modValue);
    }

    public override bool AddModifier(Modifier modifier)
    {
        if(modifiers.Contains(modifier)) return false;

        modifiers.Add(modifier);
        _modifiedDirty = true;

        if(modifier.IsTimeSensitive)
            _timeSensitiveModifiers++;
        
        return true;
    }

    public override bool RemoveModifier(Modifier modifier)
    {
        if(modifiers.Remove(modifier) == false) return false;

        _modifiedDirty = true;

        if(modifier.IsTimeSensitive)
            _timeSensitiveModifiers--;
        
        return true;
    }
}
