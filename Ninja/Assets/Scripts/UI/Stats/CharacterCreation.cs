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

    [SerializeField] private TextMeshProUGUI boonPoints;
    [SerializeField] private List<TraitElement> boonElements;
    [SerializeField] private List<TraitElement> flawElements;

    [SerializeField] private UIWindow attrPointsValidationWindow;
    [SerializeField] private UIWindow positiveBPValidationWindow;
    [SerializeField] private UIWindow negativeBPValidationWindow;
    [SerializeField] private UIWindow nextWindow;

     [SerializeField] private UIToolTip toolTip;

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
        stats.SetAttributeLevel(AttributeType.STRENGTH, attrInitial);
        stats.SetAttributeLevel(AttributeType.AGILITY, attrInitial);
        stats.SetAttributeLevel(AttributeType.DEXTERITY, attrInitial);
        stats.SetAttributeLevel(AttributeType.WISDOM, attrInitial);
    }

    public void AddStrength()
    {
        AddPoint(AttributeType.STRENGTH);
    }

    public void RemoveStrength()
    {
        RemovePoint(AttributeType.STRENGTH);
    }

    public void AddAgility()
    {
        AddPoint(AttributeType.AGILITY);
    }

    public void RemoveAgility()
    {
        RemovePoint(AttributeType.AGILITY);
    }

    public void AddDexterity()
    {
        AddPoint(AttributeType.DEXTERITY);
    }

    public void RemoveDexterity()
    {
        RemovePoint(AttributeType.DEXTERITY);
    }

    public void AddWisdom()
    {
        AddPoint(AttributeType.WISDOM);
    }

    public void RemoveWisdom()
    {
        RemovePoint(AttributeType.WISDOM);
    }

    private void AddPoint(AttributeType attributeType)
    {
        if(attrPoints < 1) return;

        if(stats.IncreaseAttribute(attributeType))
        {
            attrPoints--;
            UpdateStats();
            UpdatePoints();
        }
    }

    private void RemovePoint(AttributeType attributeType)
    {
        int value = stats.GetAttributeLevel(attributeType);
        if(value < 2)
            return;

        stats.SetAttributeLevel(attributeType, value -1);

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
            boonElements[i].SetUp(this, boons[i], TraitManager.instance.GetBoon(boons[i]).Description);

        // Deactivate leftover boon elements
        for (; i< boonElements.Count; i++)
            boonElements[i].gameObject.SetActive(false);

        i = 0;
        // Initialize flaw elements
        for (; i < flaws.Count && i < flawElements.Count; i++)
            flawElements[i].SetUp(this, flaws[i], TraitManager.instance.GetFlaw(flaws[i]).Description);

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
        strength.text = "" + stats.GetAttributeLevelModified(AttributeType.STRENGTH);
        agility.text = "" + stats.GetAttributeLevelModified(AttributeType.AGILITY);
        dexterity.text = "" + stats.GetAttributeLevelModified(AttributeType.DEXTERITY);
        wisdom.text = "" + stats.GetAttributeLevelModified(AttributeType.WISDOM);
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

    public void UpdateToolTip(string text)
    {
        toolTip.UpdateText(text);
    }

}
