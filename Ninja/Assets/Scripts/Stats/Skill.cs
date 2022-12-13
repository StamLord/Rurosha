using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Attribute
{
    public Dictionary<string, int> _prerequisite { get; private set;}
    public int _treeLevel;

    public Skill(string name, int treeLevel, Dictionary<string, int> prerequisite, int maxLevel = 1) : base(0, maxLevel)
    {
        _prerequisite = prerequisite;
        _treeLevel = treeLevel;
    }

    public override bool GainExperience(float amount)
    {
        return false;
    }

}
