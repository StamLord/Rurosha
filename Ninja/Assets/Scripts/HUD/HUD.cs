using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private QuickSpellRing quickSpellRing;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image potentialHealthBar;

    [SerializeField] private Image staminaBar;
    [SerializeField] private Image potentialStaminaBar;

    [SerializeField] private Image chakraBar;
    [SerializeField] private Sprite earthImage;
    [SerializeField] private Sprite waterImage;
    [SerializeField] private Sprite fireImage;
    [SerializeField] private Sprite windImage;
    [SerializeField] private Sprite voidImage;
    [SerializeField] private Sprite woodImage;
    [SerializeField] private Sprite metalImage;
    [SerializeField] private Sprite lightningImage;
    [SerializeField] private Sprite iceImage;

    [SerializeField] private Image lowHealth;

    [Header("Low Health Settings")]
    [SerializeField] private float lowHealthThreshold = .2f;
    [SerializeField] private float showLowHealthDuration = .2f;
    [SerializeField] private float hideLowHealthDuration = 1f;
    [SerializeField] private float lowHealthPulseRate = 1f;
    [SerializeField] private float lowHealthPulseMinAlpha = .7f;

    private Coroutine animLowHealth;
    private bool isAnimLowHealth;
    private bool lowHealthDisplayed;
    private ChakraType chakraType;
    private bool isVisible;

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        if(isVisible)
        {
            if(healthBar) healthBar.gameObject.SetActive(true);
            if(potentialHealthBar) potentialHealthBar.gameObject.SetActive(true);
            if(staminaBar) staminaBar.gameObject.SetActive(true);
            if(potentialStaminaBar) potentialStaminaBar.gameObject.SetActive(true);
        }
        else
        {
            if(healthBar) healthBar.gameObject.SetActive(false);
            if(potentialHealthBar) potentialHealthBar.gameObject.SetActive(false);
            if(staminaBar) staminaBar.gameObject.SetActive(false);
            if(potentialStaminaBar) potentialStaminaBar.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if(healthBar)
        {
            playerStats.HealthUpdateEvent += UpdateHealthBar;
            healthBar.fillAmount = playerStats.Health / playerStats.MaxHealth;
        }

        if(potentialHealthBar)
        {
            playerStats.PotentialHealthUpdateEvent += UpdatePotentialHealthBar;
            potentialHealthBar.fillAmount = playerStats.PotentialHealth / playerStats.MaxHealth;
        }

        if(staminaBar)
        {
            playerStats.StaminaUpdateEvent += UpdateStaminaBar;
            staminaBar.fillAmount = playerStats.Stamina / playerStats.MaxStamina;
        }

        if(potentialStaminaBar)
        {
            playerStats.PotentialStaminaUpdateEvent += UpdatePotentialStaminaBar;
            potentialStaminaBar.fillAmount = playerStats.PotentialStamina / playerStats.MaxStamina;
        }

        quickSpellRing.OnSpellChange += SpellChange;
    }
    
    private void Update() 
    {
        // Pulsate low health overlay
        if(lowHealthDisplayed && isAnimLowHealth == false)
        {
            float t = (Mathf.Sin(Time.time * lowHealthPulseRate) + 1) / 2;
            Color color = lowHealth.color;
            color.a = Mathf.Lerp(lowHealthPulseMinAlpha, 1f, t);
            lowHealth.color = color;
        }

        if(chakraBar) chakraBar.fillAmount = playerStats.GetChakraAmount(chakraType);
    }

    private void UpdateHealthBar(float health)
    {
        healthBar.fillAmount = health;
        
        // Show low health overlay
        if(health < lowHealthThreshold && lowHealthDisplayed == false)
        {
            if(isAnimLowHealth)
                StopCoroutine(animLowHealth);
            animLowHealth = StartCoroutine("DisplayLowHealth", true);
            lowHealthDisplayed = true;
        }
        // Hide low health overlay
        else if(health > lowHealthThreshold && lowHealthDisplayed == true)
        {
            if(isAnimLowHealth)
                StopCoroutine(animLowHealth);
            animLowHealth = StartCoroutine("DisplayLowHealth", false);
            lowHealthDisplayed = false;
        }
    }

    private void UpdatePotentialHealthBar(float potentialHealth)
    {
        potentialHealthBar.fillAmount = potentialHealth;
    }

    private void UpdateStaminaBar(float stamina)
    {
        staminaBar.fillAmount = stamina;
    }

    private void UpdatePotentialStaminaBar(float potentialStamina)
    {
        potentialStaminaBar.fillAmount = potentialStamina;
    }

    private IEnumerator DisplayLowHealth(bool show)
    {
        isAnimLowHealth = true;

        float startTime = Time.time;
        float duration = (show)? showLowHealthDuration : hideLowHealthDuration;

        while(Time.time - startTime < duration)
        {
            Color color = lowHealth.color;
            color.a = (Time.time - startTime) / duration;
            if(show == false)
                color.a = 1 - color.a;

            lowHealth.color = color;

            yield return null;
        }

        isAnimLowHealth = false;
    }

    private void SpellChange(Spell spell)
    {Debug.Log("CH");
        chakraType = spell.chakraType;
        Sprite img = earthImage;
        switch(chakraType)
        {
            case ChakraType.EARTH:
                img = earthImage;
                break;
            case ChakraType.WATER:
                img = waterImage;
                break;
            case ChakraType.FIRE:
                img = fireImage;
                break;
            case ChakraType.WIND:
                img = windImage;
                break;
            case ChakraType.VOID:
                img = voidImage;
                break;
            case ChakraType.WOOD:
                img = woodImage;
                break;
            case ChakraType.METAL:
                img = metalImage;
                break;
            case ChakraType.THUNDER:
                img = lightningImage;
                break;
            case ChakraType.ICE:
                img = iceImage;
                break;
        }
        chakraBar.sprite = img;
    }
}
