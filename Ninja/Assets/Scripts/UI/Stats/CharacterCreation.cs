using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private TextMeshProUGUI boonPoints;
    [SerializeField] private List<TraitElement> boonElements;
    [SerializeField] private List<TraitElement> flawElements;

    [SerializeField] private UIWindow attrPointsValidationWindow;
    [SerializeField] private UIWindow positiveBPValidationWindow;
    [SerializeField] private UIWindow negativeBPValidationWindow;
    [SerializeField] private UIWindow nextWindow;

    [Header("Settings")]
    [SerializeField] private int attrPoints = 10;
    [SerializeField] private int attrInitial = 4;

    [SerializeField] private int bpPoints = 1;

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
        InitializeTraits();
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

    private void InitializeTraits()
    {
        List<string> boons = TraitManager.instance.GetBoonList();
        List<string> flaws = TraitManager.instance.GetFlawList();

        int i = 0;
        // Initialize boon elements
        for (; i < boons.Count && i < boonElements.Count; i++)
            boonElements[i].SetUp(this, boons[i]);

        // Deactivate leftover boon elements
        for (; i< boonElements.Count; i++)
            boonElements[i].gameObject.SetActive(false);

        i = 0;
        // Initialize flaw elements
        for (; i < flaws.Count && i < flawElements.Count; i++)
            flawElements[i].SetUp(this, flaws[i]);

        // Deactivate leftover flaw elements
        for (; i< flawElements.Count; i++)
            flawElements[i].gameObject.SetActive(false);

    }

    public bool ToggleBoon(string boon, bool state)
    {
        if(state)
        {
            if(bpPoints < 1) return false;

            stats.AddBoon(boon);
            bpPoints--;
        }
        else
        {
            stats.RemoveBoon(boon);
            bpPoints++;
        }

        UpdatePoints();
        UpdateStats();
        return true;
    }
    
    public bool ToggleFlaw(string flaw, bool state)
    {
        if(state)
        {
            stats.AddFlaw(flaw);
            bpPoints++;
        }
        else
        {
            stats.RemoveFlaw(flaw);
            bpPoints--;
        }

        UpdatePoints();
        UpdateStats();
        return true;
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
        boonPoints.text = "" + bpPoints;
    }

    private bool ValidateCreation()
    {
        if(attrPoints > 0) // Unused attribute points
        {
            attrPointsValidationWindow.Open();
            return false;
        } 

        if(bpPoints > 0) // Unused boon points
        {
            positiveBPValidationWindow.Open();
            return false;
        } 

        if(bpPoints < 0) // Too many boons vs flaws
        {
            negativeBPValidationWindow.Open();
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
