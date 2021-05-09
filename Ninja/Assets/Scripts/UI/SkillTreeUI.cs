using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    public SkillTree tree;
    public GameObject skillPrefab;

    public TextMeshProUGUI treeLevel;
    public TextMeshProUGUI points;
    public Image treeExp;

    public float levelHeight;
    public float skillSpace;

    void Start()
    {
        //InitializeTree();
    }

    void InitializeTree()
    {
        Dictionary<int, List<Skill>> skills = tree.GetOrderedSkills();

        for (int i = 0; i < skills.Count; i++)
        {
            for (int j = 0; j < skills[i].Count; j++)
            {
                GameObject go = Instantiate(skillPrefab, new Vector3(j * skillSpace, i * levelHeight, 0), Quaternion.identity, transform);
            }
        }
    }

    void UpdateTree()
    {
        treeExp.fillAmount = tree._experience;
        treeLevel.text = "" + tree._level;
    }
}
