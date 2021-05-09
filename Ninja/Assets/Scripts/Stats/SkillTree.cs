using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : Attribute
{
    private int _points;
    private List<Skill> _skills = new List<Skill>();

    public SkillTree(string name, List<Skill> skills, int minLevel = 1) : base(name, minLevel, TotalLevels(skills))
    {
        _skills = skills;
    }

    private static int TotalLevels(List<Skill> skills)
    {
        int total = 0;

        foreach(Skill s in skills)
            total += s._maxLevel;

        return total;
    }

    public int HighestLevel()
    {
        int level = 0;

        foreach(Skill s in _skills)
            if(s._maxLevel > level)
                level = s._maxLevel;

        return level;
    }

    public Dictionary<int, List<Skill>> GetOrderedSkills()
    {
        Dictionary<int, List<Skill>> ordered = new Dictionary<int, List<Skill>>();

        foreach(Skill s in _skills)
        {
            int tLevel = s._treeLevel;

            if(ordered[tLevel] == null)
                ordered.Add(tLevel, new List<Skill>());

            ordered[tLevel].Add(s);
        }

        return ordered;
    }

    public override bool IncreaseLevel()
    {
        if(base.IncreaseLevel())
        {
            _points++;
            return true;
        }

        return false;
    }
}
