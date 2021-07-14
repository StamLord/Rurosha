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

    [SerializeField] private GameObject statWindow;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
            SetWindow(!statWindow.activeSelf);

        if(statWindow.activeSelf)
            RefreshWindow();
    }

    void SetWindow(bool open)
    {
        statWindow.SetActive(open);
        
        if(open)
            UIManager.Instance.AddWindow(this);
        else
            UIManager.Instance.RemoveWindow(this);
    }

    void RefreshWindow()
    {
        health.text = Mathf.RoundToInt(characterStats.Health) + "/" + (characterStats.MaxHealth);

        stamina.text =  Mathf.RoundToInt(characterStats.Stamina) + "/" + (characterStats.MaxStamina);

        strength.text =  "" + characterStats.GetAttributeLevel("strength");
        endurance.text =  "" + characterStats.GetAttributeLevel("endurance");
        agility.text =  "" + characterStats.GetAttributeLevel("agiliTy");
        dexterity.text =  "" + characterStats.GetAttributeLevel("dexterity");
        mind.text =  "" + characterStats.GetAttributeLevel("mind");

        strengthExp.fillAmount = characterStats.GetAttributeExp("strength");
        enduranceExp.fillAmount = characterStats.GetAttributeExp("endurance");
        agilityExp.fillAmount = characterStats.GetAttributeExp("agiliTy");
        dexterityExp.fillAmount = characterStats.GetAttributeExp("dexterity");
        mindExp.fillAmount = characterStats.GetAttributeExp("mind");
    }

    public void IncreaseAttributeLevel(string attribute)
    {
        characterStats.IncreaseAttribute(attribute);
    }
}
