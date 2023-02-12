using UnityEngine;

[System.Serializable]
public class Skill
{
    [SerializeField] private string skillName;
    public string SkillName { get { return skillName; }}

    [SerializeField] private bool learned;
    public bool Learned { get { return learned; }}

    public string[] requirements;
    public SkillTree context;

    public bool CanLearn()
    {
        if(context == null) return true;

        foreach (string required in requirements)
        {
            if (context.IsLearned(required) == false)
                return false;
        }

        return true;
    }

    public bool Learn()
    {
        if(CanLearn() == false)
            return false;

        learned = true;
        return true;
    }

    public void Unlearn()
    {
        learned = false;
    }
}
