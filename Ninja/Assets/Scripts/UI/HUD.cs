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
        playerStats.HealthUpdateEvent += UpdateHealthBar;
        playerStats.PotentialHealthUpdateEvent += UpdatePotentialHealthBar;

        playerStats.StaminaUpdateEvent += UpdateStaminaBar;
        playerStats.PotentialStaminaUpdateEvent += UpdatePotentialStaminaBar;

        healthBar.fillAmount = playerStats.Health / playerStats.MaxHealth;
        potentialHealthBar.fillAmount = playerStats.Health / playerStats.MaxHealth;

        staminaBar.fillAmount = playerStats.Stamina / playerStats.MaxStamina;
        potentialStaminaBar.fillAmount = playerStats.Stamina / playerStats.MaxStamina;
    }

    void UpdateHealthBar(float health)
    {
        healthBar.fillAmount = health;
        //if(delta != 0) 
        //StartHealthAftermath();
    }

    void UpdatePotentialHealthBar(float potentialHealth)
    {
        potentialHealthBar.fillAmount = potentialHealth;
    }

    void UpdateStaminaBar(float stamina)
    {
        staminaBar.fillAmount = stamina;
        //StartStaminaAftermath();
    }

    void UpdatePotentialStaminaBar(float potentialStamina)
    {
        potentialStaminaBar.fillAmount = potentialStamina;
    }

    void UpdatePotentialHealth(float delta)
    {

    }

    // void StartHealthAftermath()
    // {
    //     if(hAftermathDone == false) return;

    //     hAftermathStartTime = Time.time;
    //     hAftermathDone = false;
    // }

    // void UpdateHealthAftermath()
    // {
    //     if(hAftermathDone) return;

    //     float hBar = healthBar.fillAmount;
    //     float haBar = potentialHealthBar.fillAmount;
        
    //     if(hBar == haBar) 
    //     {
    //         hAftermathDone = true;
    //         return;
    //     }

    //     if((Time.time - hAftermathStartTime) > hAftermathDelay)
    //     {
    //         float speed =  1 / hAftermathSpeed * Time.deltaTime;
    //         float direction = ((hBar - haBar) > 0)? 1 : -1;

    //         float newValue = haBar + speed * direction;
    //         newValue = (direction == 1)? Mathf.Min(newValue, hBar) : Mathf.Max(newValue, hBar);

    //         potentialHealthBar.fillAmount = newValue;
    //     }
    // }

    // void StartStaminaAftermath()
    // {
    //     if(sAftermathDone == false) return;

    //     sAftermathStartTime = Time.time;
    //     sAftermathDone = false;
    // }

    // void UpdateStaminaAftermath()
    // {
    //     if(sAftermathDone) return;

    //     float sBar = staminaBar.fillAmount;
    //     float saBar = potentialStaminaBar.fillAmount;
        
    //     if(sBar == saBar) 
    //     {
    //         sAftermathDone = true;
    //         return;
    //     }

    //     if((Time.time - sAftermathStartTime) > sAftermathDelay)
    //     {
    //         float speed =  1 / hAftermathSpeed * Time.deltaTime;
    //         float direction = ((sBar - saBar) > 0)? 1 : -1;

    //         float newValue = saBar + speed * direction;
    //         newValue = (direction == 1)? Mathf.Min(newValue, sBar) : Mathf.Max(newValue, sBar);

    //        potentialStaminaBar.fillAmount = newValue;
    //     }

    // }
}
