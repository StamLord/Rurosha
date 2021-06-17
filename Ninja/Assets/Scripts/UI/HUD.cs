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

    [SerializeField] private float hAftermathDelay = 3f;
    [SerializeField] private float hAftermathSpeed = 1f;
    [SerializeField] private float hAftermathStartTime;
    [SerializeField] private bool hAftermathDone;

    [SerializeField] private float sAftermathDelay = 3f;
    [SerializeField] private float sAftermathSpeed = 1f;
    [SerializeField] private float sAftermathStartTime;
    [SerializeField] private bool sAftermathDone;

    void Start()
    {
        #region Events Register

        playerStats.HealthUpdateEvent += UpdateHealthBar;
        playerStats.PotentialHealthUpdateEvent += UpdatePotentialHealthBar;

        playerStats.StaminaUpdateEvent += UpdateStaminaBar;
        playerStats.PotentialStaminaUpdateEvent += UpdatePotentialStaminaBar;

        #endregion

        healthBar.fillAmount = playerStats.Health / playerStats.MaxHealth;
        potentialHealthBar.fillAmount = playerStats.PotentialHealth / playerStats.MaxHealth;

        staminaBar.fillAmount = playerStats.Stamina / playerStats.MaxStamina;
        potentialStaminaBar.fillAmount = playerStats.potentialStamina / playerStats.MaxStamina;
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
