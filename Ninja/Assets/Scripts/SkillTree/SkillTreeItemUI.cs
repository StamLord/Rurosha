using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillTreeItemUI : MonoBehaviour, IPointerClickHandler//, IPointerEnterHandler
{
    public SkillTree tree;
    public string skillName;

    public TextMeshProUGUI skillText;
    public Image skillLearned;
    public Image skillNotLearned;

    private void Start() 
    {
        tree.OnTreeUpdate += UpdateUI;
    }

    private void OnValidate() 
    {
        gameObject.name = skillName;
        skillText.text = skillName;    
    }

    public void UpdateUI()
    {
        bool learned = tree.IsLearned(skillName);

        if(skillLearned) skillLearned.enabled = learned;
        if(skillNotLearned) skillNotLearned.enabled = !learned;
    }

    public void OnPointerClick(PointerEventData eventData)
    {   
        if(tree.IsLearned(skillName))
            tree.Unlearn(skillName);
        else
            tree.Learn(skillName);

        UpdateUI();
    }

    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     throw new System.NotImplementedException();
    // }
}
