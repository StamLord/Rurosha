using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute
{
    [SerializeField] private string _name;
    [SerializeField] private int _level;
    [SerializeField] private int _minLevel;
    [SerializeField] private int _maxLevel;
    [SerializeField] private float _experience;

    public string Name {get {return _name;}}
    public int Level {get {return _level;}}
    public int MinLevel {get {return _minLevel;}}
    public int MaxLevel {get {return _maxLevel;}}
    public float Experience {get {return _experience;}}

    public Attribute(string name, int minLevel = 1, int maxLevel = 10)
    {
        _name = name;
        _minLevel = minLevel;
        _level = minLevel;
        _maxLevel = maxLevel;
    }

    public virtual bool IncreaseLevel()
    {
        if(_level != _maxLevel)
        {
            _level++;
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
    }
}
