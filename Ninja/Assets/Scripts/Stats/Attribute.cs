using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute
{
    public string _name;// { get; private set;}
    public int _level;// { get; private set;}
    public int _maxLevel;// { get; private set;}
    public float _experience;

    public Attribute(string name, int minLevel = 1, int maxLevel = 10)
    {
        _name = name;
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
}
