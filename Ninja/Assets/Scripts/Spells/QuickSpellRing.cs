using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickSpellRing : UIWindow
{
    [Header("References")]
    
    [SerializeField] private SpellManager spellManager;
    [SerializeField] private InputState inputState;
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform selectionImage;
    [SerializeField] private TextMeshProUGUI[] spellText;
    [SerializeField] private Image[] spellType;

    [SerializeField] private Sprite earthIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite windIcon;
    [SerializeField] private Sprite voidIcon;
    [SerializeField] private Sprite woodIcon;
    [SerializeField] private Sprite metalIcon;
    [SerializeField] private Sprite lightningIcon;
    [SerializeField] private Sprite iceIcon;

    [Header("Ring Settings")]
    [SerializeField] private int selectionsNum = 8;
    [SerializeField] private float pressTimeToOpen = 1f;
    [SerializeField] private float minDistanceFromCenter = 10f;

    [Header("Slow Down")]
    [SerializeField] private float timeScale = .2f;

    private int selected;
    private bool visible;
    private float prevTimeScale;

    private void Update()
    {
        if(visible)
        {
            GetSelection();
            UpdateSelectionImage();
            if(inputState.Spell.State == VButtonState.PRESS_END)
                HideRing();
        }
        else
        {
            if(inputState.Spell.State == VButtonState.PRESS_END)
                spellManager.Cast(selected);
            if(inputState.Spell.State == VButtonState.PRESSED && inputState.Spell.PressTime > pressTimeToOpen)
                ShowRing();
        }
    }

    private void GetSelection()
    {
        Vector3 screenCenter = new Vector3(Screen.width * .5f, Screen.height * .5f, 0);
        Vector2 mousePos = Input.mousePosition - screenCenter;

        // Make sure we are far enough from center of screen to make any selection
        if(mousePos.magnitude < minDistanceFromCenter)
            return;
        
        float angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
        if(angle < 0)
             angle += 360;
        
        selected = Mathf.FloorToInt(angle / (360 / selectionsNum));
    }

    private void UpdateSelectionImage()
    {
        selectionImage.localRotation = Quaternion.Euler(0, 0, selected * -45);
    }

    private void UpdateSpellNames()
    {
        for(int i = 0; i < spellText.Length; i++)
            spellText[i].text = spellManager.GetPreparedSpellName(i);
    }

    private void UpdateSpellTypes()
    {
        for(int i = 0; i < spellType.Length; i++)
        {
            Spell spell = spellManager.GetPreparedSpell(i);
            if(spell == null)
            {
                spellType[i].enabled = false;
                continue;
            }
            else
                spellType[i].enabled = true;

            Sprite img = null;
            switch(spell.chakraType)
            {
                case ChakraType.EARTH:
                    img = earthIcon;
                    break;
                case ChakraType.WATER:
                    img = waterIcon;
                    break;
                case ChakraType.FIRE:
                    img = fireIcon;
                    break;
                case ChakraType.WIND:
                    img = windIcon;
                    break;
                case ChakraType.VOID:
                    img = voidIcon;
                    break;
                case ChakraType.WOOD:
                    img = woodIcon;
                    break;
                case ChakraType.METAL:
                    img = metalIcon;
                    break;
                case ChakraType.THUNDER:
                    img = lightningIcon;
                    break;
                case ChakraType.ICE:
                    img = iceIcon;
                    break;
            }
            spellType[i].sprite = img;
        }
    }

    private void ShowRing()
    {
        UpdateSpellNames();
        UpdateSpellTypes();
        animator.Play("show");
        visible = true;
        UIManager.Instance.AddWindow(this, false, true, true);
        prevTimeScale = Time.timeScale;
        Time.timeScale = timeScale;
    }

    private void HideRing()
    {
        animator.Play("hide");
        visible = false;
        UIManager.Instance.RemoveWindow(this, false, true, true);
        Time.timeScale = prevTimeScale;
    }
}
