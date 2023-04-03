using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : Modifieable
{
    [SerializeField] private float value;
    [SerializeField] private float modified;
    
    public float Value {get {return value;}}
    public float Modified {get {return modified;}}

    public override int CalculateModified()
    {
        // Apply all modifiers
        float modified = value;

        for (int i = 0; i < modifiers.Count; i++)
        {
            modified = modifiers[i].Process(modified, DayNightManager.instance.GetDayTime());
        }

        // Round to whole number
        switch(roundType)
        {
            case RoundType.DOWN:
                return Mathf.FloorToInt(modified);
            case RoundType.UP:
                return Mathf.CeilToInt(modified);
        }

        return Mathf.RoundToInt(modified);
    }

    public override bool AddModifier(Modifier modifier)
    {
        if(modifiers.Contains(modifier)) return false;

        modifiers.Add(modifier);
        modified = CalculateModified();
        return true;
    }

    public override bool RemoveModifier(Modifier modifier)
    {
        if(modifiers.Contains(modifier) == false) return false;

        modifiers.Remove(modifier);
        modified = CalculateModified();
        return true;
    }
}
