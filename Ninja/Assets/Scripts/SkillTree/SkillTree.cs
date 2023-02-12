using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [Tooltip("All skills under this tree")]
    [SerializeField] private Skill[] skills;

    // Holds ref to all branches and their skillName
    private Dictionary<string, Skill> skillCache;
    
    // Holds all children (reverse of requirement) of every skill
    private Dictionary<string, List<Skill>> childrenCache;
    
    // Flag used to know when to send tree update event
    private bool isDirty;
    public delegate void TreeUpdateDelegate();
    public event TreeUpdateDelegate OnTreeUpdate;

    public void Start()
    {
        InitializeSkills();
    }

    private void Update() 
    {
        if(isDirty)
        {
            if(OnTreeUpdate != null)
                OnTreeUpdate();
            
            isDirty = false;
        }
    }

    // <summary>
    /// Gets all branches and fills the cache for easy checking later
    /// </summary>
    public void InitializeSkills()
    {
        skillCache = new Dictionary<string, Skill>();
        childrenCache = new Dictionary<string, List<Skill>>();

        if(skills.Length < 1) return;

        foreach (Skill s in skills)
        {
            s.context = this;
            skillCache[s.SkillName] = s;

            // Fill childrenCache (reverse of requirement)
            foreach (string required in s.requirements)
            {
                if(childrenCache.ContainsKey(required) == false)
                    childrenCache[required] = new List<Skill>();
                
                childrenCache[required].Add(s);
            }
        }
    }

    public Skill GetSkill(string skillName)
    {
        if(skillCache.ContainsKey(skillName))
            return skillCache[skillName];
        
        return null;
    }

    /// <summary>
    /// Checks if a skill is learned (it's branch is active)
    /// </summary>
    public bool IsLearned(string skillName)
    {
        if(skillCache.ContainsKey(skillName))
            return skillCache[skillName].Learned;
        
        return false;
    }

    public bool Learn(string skillName)
    {
        if(skillCache.ContainsKey(skillName))
            isDirty = skillCache[skillName].Learn();
        
        return false;        
    }

    public void Unlearn(string skillName)
    {
        if(IsLearned(skillName) == false)
            return;
        
        if(skillCache.ContainsKey(skillName))
        {
            skillCache[skillName].Unlearn();

            if(childrenCache.ContainsKey(skillName))
            {
                foreach (Skill s in childrenCache[skillName])
                    Unlearn(s.SkillName);
            }

            isDirty = true;
        }
    }
}
