using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Image healthBar;
    public Image potentialHealthBar;

    public Image staminaBar;
    public Image potentialStaminaBar;

    public CharacterStats playerStats;

    void Start()
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
            potentialStaminaBar.fillAmount = playerStats.potentialStamina / playerStats.MaxStamina;
        }
    }

    void UpdateHealthBar(float health)
    {
        healthBar.fillAmount = health;
    }

    void UpdatePotentialHealthBar(float potentialHealth)
    {
        potentialHealthBar.fillAmount = potentialHealth;
    }

    void UpdateStaminaBar(float stamina)
    {
        staminaBar.fillAmount = stamina;
    }

    void UpdatePotentialStaminaBar(float potentialStamina)
    {
        potentialStaminaBar.fillAmount = potentialStamina;
    }
}
