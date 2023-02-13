using UnityEngine;
using TMPro;

public class SkillPointsDisplay : MonoBehaviour
{
    [SerializeField] private SkillTreeManager skillTreeManager;
    [SerializeField] private TextMeshProUGUI text;

    private void Update() 
    {
        text.text = "" + skillTreeManager.SkillPoints;
    }
}
