using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifieable
{
    public enum RoundType {DOWN, UP}
    [SerializeField] protected RoundType roundType;
    [SerializeField] protected List<Modifier> modifiers = new List<Modifier>();

    [SerializeField] protected int _modified;
    [SerializeField] protected bool _modifiedDirty = true;
    [SerializeField] protected int _timeSensitiveModifiers = 0;

    public int Modified {
        get 
        {
            if(_modifiedDirty)
            {
                _modified = CalculateModified();
                _modifiedDirty = false;
            }
            
            return _modified;
        }}

    public abstract int CalculateModified();

    public abstract bool AddModifier(Modifier modifier);

    public abstract bool RemoveModifier(Modifier modifier);

    public void TimeBasedCalculateModified(DayNightManager.DayTime time)
    {
        if(_timeSensitiveModifiers > 0)
            _modifiedDirty = true;
    }
}
