using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterCreation : UIWindow
{
    [Header("References")]
    [SerializeField] private CharacterStats stats;
    [SerializeField] private TextMeshProUGUI attributePoints;
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI agility;
    [SerializeField] private TextMeshProUGUI dexterity;
    [SerializeField] private TextMeshProUGUI wisdom;

    [SerializeField] private UIWindow attrPointsValidationWindow;
    [SerializeField] private UIWindow nextWindow;

    [Header("Settings")]
    [SerializeField] private int attrPoints = 10;
    [SerializeField] private int attrInitial = 4;

    [SerializeField] private bool openOnStart = false;

    private string charName;

    private void Start() 
    {
        DebugCommandDatabase.AddCommand(new DebugCommand(
            "showcharmenu", 
            "Shows character creation menu", 
            "showcharmenu",
            (string[] parameters) => 
            {
                this.Open();
                return "";
            }));

        SetInitialAttributes();
        UpdatePoints();
        UpdateStats();

        if(openOnStart) Open();
    }

    private void SetInitialAttributes()
    {
        stats.SetAttributeLevel("strength", attrInitial);
        stats.SetAttributeLevel("agility", attrInitial);
        stats.SetAttributeLevel("dexterity", attrInitial);
        stats.SetAttributeLevel("wisdom", attrInitial);
    }

    public void AddPoint(string attributeName)
    {
        if(attrPoints < 1) return;

        if(stats.IncreaseAttribute(attributeName))
        {
            attrPoints--;
            UpdateStats();
            UpdatePoints();
        }
    }

    public void RemovePoint(string attributeName)
    {
        int value = stats.GetAttributeLevel(attributeName);
        if(value < 2)
            return;

        stats.SetAttributeLevel(attributeName, value -1);

        attrPoints++;
        UpdateStats();
        UpdatePoints();
    }

    private void UpdateStats()
    {
        strength.text = "" + stats.GetAttributeLevel("strength");
        agility.text = "" + stats.GetAttributeLevel("agility");
        dexterity.text = "" + stats.GetAttributeLevel("dexterity");
        wisdom.text = "" + stats.GetAttributeLevel("wisdom");
    }

    private void UpdatePoints()
    {
        attributePoints.text = "" + attrPoints;        
    }

    private bool ValidateCreation()
    {
        if(attrPoints > 0)
        {
            attrPointsValidationWindow.Open();
            return false;
        } 

        return true;
    }

    public void Done()
    {
        if(ValidateCreation() == false) return;

        if(nextWindow) nextWindow.Open();
        Close();
    }

}
