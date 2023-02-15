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
        if(mobility) 
        {
            mobility.SetManager(this);
            mobility.OnSkillLearned += SkillLearned;
        }

        if(stealth) 
        {
            stealth.SetManager(this);
            stealth.OnSkillLearned += SkillLearned;
        }
        if(survival)
        { 
            survival.SetManager(this);
            survival.OnSkillLearned += SkillLearned;
        }

        if(spirit) 
        {
            spirit.SetManager(this);
            spirit.OnSkillLearned += SkillLearned;
        }

        if(unarmed) 
        { 
            unarmed.SetManager(this);
            unarmed.OnSkillLearned += SkillLearned;
        }
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

    public bool IsLearned(string skillName)
    {
        if(mobility && mobility.IsLearned(skillName))
            return true;
        
        if(stealth && stealth.IsLearned(skillName))
            return true;
        
        if(survival && survival.IsLearned(skillName))
            return true;
        
        if(spirit && spirit.IsLearned(skillName))
            return true;

        if(unarmed && unarmed.IsLearned(skillName))
            return true;
        
        return false;
    }

    public void SkillLearned(string skillName)
    {
        switch(skillName)
        {
            case "Sense Money":
                SenseMoneyVFX.Active = true;
                break;
        }
    }
}
