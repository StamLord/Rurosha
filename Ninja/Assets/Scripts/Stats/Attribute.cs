using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute : Modifieable
{
    [SerializeField] private int _level;
    [SerializeField] private int _minLevel;
    [SerializeField] private int _maxLevel;
    [SerializeField] private float _experience;
    
    public int Level {get {return _level;}}
    public int MinLevel {get {return _minLevel;}}
    public int MaxLevel {get {return _maxLevel;}}
    public float Experience {get {return _experience;}}
   
    public Attribute(int minLevel = 1, int maxLevel = 10)
    {
        _minLevel = minLevel;
        _level = minLevel;
        _maxLevel = maxLevel;
        _modified = _level;

        DayNightManager.instance.OnHourPassed += TimeBasedCalculateModified;
    }

    public void Initialize(int minLevel = 1, int maxLevel = 10)
    {
        _minLevel = minLevel;
        _maxLevel = maxLevel;
        _level = Mathf.Clamp(_level, _minLevel, _maxLevel);
        _modified = CalculateModified();
        _modifiedDirty = false;

        DayNightManager.instance.OnHourPassed += TimeBasedCalculateModified;
    }

    public virtual bool IncreaseLevel()
    {
        if(_level != _maxLevel)
        {
            _level++;
            _modifiedDirty = true;
            return true;
        }

        return false;
    }

    public virtual bool GainExperience(float amount)
    {
        if(_level != _maxLevel)
        {
            _experience += amount;
            if(_experience >= 1f)
            {
                _experience %= 1f;
                return IncreaseLevel();
            }
        }

        return false;
    }

    public virtual void SetLevel(int level)
    {
        _level = Mathf.Clamp(level, _minLevel, _maxLevel);
        _modifiedDirty = true;
    }

    public override int CalculateModified()
    {
        if(modifiers.Count < 1) return Level;

        // Apply all modifiers
        float modValue = Level;
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
        if(modifiers.Contains(modifier) == false) return false;

        modifiers.Remove(modifier);
        _modifiedDirty = true;

        if(modifier.IsTimeSensitive)
            _timeSensitiveModifiers--;
        
        return true;
    }
}
