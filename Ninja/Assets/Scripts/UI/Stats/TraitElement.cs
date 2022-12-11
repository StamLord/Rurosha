using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TraitElement : MonoBehaviour, IPointerEnterHandler
{
    public enum TraitType {BOON, FLAW};

    [SerializeField] private bool toggled;
    [SerializeField] private TraitType type;
    [SerializeField] private Image toggleIcon;
    [SerializeField] private TextMeshProUGUI textName;

    private CharacterCreation context;
    private string description;

    public void SetUp(CharacterCreation context, string name, string description)
    {
        this.context = context;
        textName.text = name;
        this.description = description;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        context.UpdateToolTip(description);
    }

}
