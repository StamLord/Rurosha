using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraitElement : MonoBehaviour
{
    public enum TraitType {BOON, FLAW};

    [SerializeField] private bool toggled;
    [SerializeField] private TraitType type;
    [SerializeField] private Image toggleIcon;
    [SerializeField] private TextMeshProUGUI textName;

    private CharacterCreation context;

    public void SetUp(CharacterCreation context, string name)
    {
        this.context = context;
        textName.text = name;
    }

    public void Toggle()
    {
        switch(type)
        {
            case TraitType.BOON:
                if(context.ToggleBoon(textName.text, !toggled))
                    toggled = !toggled;
                break;
            case TraitType.FLAW:
                if(context.ToggleFlaw(textName.text, !toggled))
                    toggled = !toggled;
                break;
        }
        
        UpdateToggleVisual();
    }

    private void UpdateToggleVisual()
    {
        toggleIcon.enabled = toggled;
    }

}
