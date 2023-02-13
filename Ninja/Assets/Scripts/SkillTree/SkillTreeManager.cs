using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] private int skillPoints;
    public int SkillPoints { get{ return skillPoints; }}

    [Header("Skill Trees")]
    [SerializeField] private SkillTree mobility;
    [SerializeField] private SkillTree stealth;
    [SerializeField] private SkillTree survival;
    [SerializeField] private SkillTree spirit;
    [SerializeField] private SkillTree unarmed;

    private void Start() 
    {
        mobility.SetManager(this);
        stealth.SetManager(this);
        survival.SetManager(this);
        spirit.SetManager(this);
        unarmed.SetManager(this);
    }

    public void AddSkillPoint(int amount)
    {
        skillPoints += amount;
    }

    public bool RemoveSkillPoint(int amount)
    {
        if(amount > skillPoints) return false;

        skillPoints -= amount;
        return true;
    }
}
