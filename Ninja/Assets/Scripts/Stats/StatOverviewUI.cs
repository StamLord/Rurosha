using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatOverviewUI : UIWindow
{
    [SerializeField] private CharacterStats characterStats;

    [SerializeField] private TextMeshProUGUI health;

    [SerializeField] private TextMeshProUGUI stamina;

    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI endurance;
    [SerializeField] private TextMeshProUGUI agility;
    [SerializeField] private TextMeshProUGUI dexterity;
    [SerializeField] private TextMeshProUGUI mind;

    [SerializeField] private Image strengthExp;
    [SerializeField] private Image enduranceExp;
    [SerializeField] private Image agilityExp;
    [SerializeField] private Image dexterityExp;
    [SerializeField] private Image mindExp;

    public override void RefreshWindow()
    {
        health.text = Mathf.RoundToInt(characterStats.Health) + "/" + (characterStats.MaxHealth);

        stamina.text =  Mathf.RoundToInt(characterStats.Stamina) + "/" + (characterStats.MaxStamina);

        strength.text =  "" + characterStats.GetAttributeLevelModified(AttributeType.STRENGTH);
        agility.text =  "" + characterStats.GetAttributeLevelModified(AttributeType.AGILITY);
        dexterity.text =  "" + characterStats.GetAttributeLevelModified(AttributeType.DEXTERITY);
        mind.text =  "" + characterStats.GetAttributeLevelModified(AttributeType.WISDOM);

        strengthExp.fillAmount = characterStats.GetAttributeExp(AttributeType.STRENGTH);
        agilityExp.fillAmount = characterStats.GetAttributeExp(AttributeType.AGILITY);
        dexterityExp.fillAmount = characterStats.GetAttributeExp(AttributeType.DEXTERITY);
        mindExp.fillAmount = characterStats.GetAttributeExp(AttributeType.WISDOM);
    }

    public void IncreaseAttributeLevel(AttributeType attribute)
    {
        characterStats.IncreaseAttribute(attribute);
    }
}
