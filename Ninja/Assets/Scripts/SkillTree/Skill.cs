using UnityEngine;

[System.Serializable]
public class Skill
{
    [SerializeField] private string skillName;
    public string SkillName { get { return skillName; }}

    [SerializeField] private int cost = 1;
    public int Cost { get { return cost; }}

    [SerializeField] private bool learned;
    public bool Learned { get { return learned; }}

    public string[] requirements;

    public void Learn()
    {
        learned = true;
    }

    public void Unlearn()
    {
        learned = false;
    }
}
