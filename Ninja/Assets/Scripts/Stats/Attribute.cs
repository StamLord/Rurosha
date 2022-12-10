using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute : Modifieable
{
    [SerializeField] private string _name;
    [SerializeField] private int _level;
    [SerializeField] private int _minLevel;
    [SerializeField] private int _maxLevel;
    [SerializeField] private float _experience;
    [SerializeField] private int _modified;

    public string Name {get {return _name;}}
    public int Level {get {return _level;}}
    public int MinLevel {get {return _minLevel;}}
    public int MaxLevel {get {return _maxLevel;}}
    public float Experience {get {return _experience;}}
    public int Modified {get {return _modified;}}

    public Attribute(string name, int minLevel = 1, int maxLevel = 10)
    {
        _name = name;
        _minLevel = minLevel;
        _level = minLevel;
        _maxLevel = maxLevel;
        _modified = _level;
    }

    public virtual bool IncreaseLevel()
    {
        if(_level != _maxLevel)
        {
            _level++;
            _modified = CalculateModified();
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
        _modified = CalculateModified();
    }

    public override int CalculateModified()
    {
        if(modifiers.Count < 1) return Level;

        // Apply all modifiers
        float modified = Level;

        for (int i = 0; i < modifiers.Count; i++)
            modified = modifiers[i].Process(modified);

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
        _modified = CalculateModified();
        return true;
    }

    public override bool RemoveModifier(Modifier modifier)
    {
        if(modifiers.Contains(modifier) == false) return false;

        modifiers.Remove(modifier);
        _modified = CalculateModified();
        return true;
    }
}
