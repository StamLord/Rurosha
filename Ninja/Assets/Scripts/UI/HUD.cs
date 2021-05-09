using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public Material healthBarMat;
    public Material healthBarAftermathMat;
    public Material staminaBarMat;
    public Material staminaBarAftermathMat;

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
        playerStats.StaminaUpdateEvent += UpdateStaminaBar;

        healthBarMat.SetFloat("_Fill", playerStats.health / playerStats.maxHealth);
        healthBarAftermathMat.SetFloat("_Fill", playerStats.health / playerStats.maxHealth);

        staminaBarMat.SetFloat("_Fill", playerStats.stamina / playerStats.maxStamina);
        staminaBarAftermathMat.SetFloat("_Fill", playerStats.stamina / playerStats.maxStamina);
    }

    void LateUpdate()
    {
        UpdateHealthAftermath();
        UpdateStaminaAftermath();
    }

    void UpdateHealthBar(float delta)
    {
        healthBarMat.SetFloat("_Fill", playerStats.health / playerStats.maxHealth);
        //if(delta != 0) 
        StartHealthAftermath();
    }

    void UpdateStaminaBar(float delta)
    {
        staminaBarMat.SetFloat("_Fill", playerStats.stamina / playerStats.maxStamina);
        StartStaminaAftermath();
    }

    void StartHealthAftermath()
    {
        if(hAftermathDone == false) return;

        hAftermathStartTime = Time.time;
        hAftermathDone = false;
    }

    void UpdateHealthAftermath()
    {
        if(hAftermathDone) return;

        float hBar = healthBarMat.GetFloat("_Fill");
        float haBar = healthBarAftermathMat.GetFloat("_Fill");
        
        if(hBar == haBar) 
        {
            hAftermathDone = true;
            return;
        }

        if((Time.time - hAftermathStartTime) > hAftermathDelay)
        {
            float speed =  1 / hAftermathSpeed * Time.deltaTime;
            float direction = ((hBar - haBar) > 0)? 1 : -1;

            float newValue = haBar + speed * direction;
            newValue = (direction == 1)? Mathf.Min(newValue, hBar) : Mathf.Max(newValue, hBar);

            healthBarAftermathMat.SetFloat("_Fill", newValue);
        }
    }

    void StartStaminaAftermath()
    {
        if(sAftermathDone == false) return;

        sAftermathStartTime = Time.time;
        sAftermathDone = false;
    }

    void UpdateStaminaAftermath()
    {
        if(sAftermathDone) return;

        float sBar = staminaBarMat.GetFloat("_Fill");
        float saBar = staminaBarAftermathMat.GetFloat("_Fill");
        
        if(sBar == saBar) 
        {
            sAftermathDone = true;
            return;
        }

        if((Time.time - sAftermathStartTime) > sAftermathDelay)
        {
            float speed =  1 / hAftermathSpeed * Time.deltaTime;
            float direction = ((sBar - saBar) > 0)? 1 : -1;

            float newValue = saBar + speed * direction;
            newValue = (direction == 1)? Mathf.Min(newValue, sBar) : Mathf.Max(newValue, sBar);

           staminaBarAftermathMat.SetFloat("_Fill", newValue);
        }

    }
}
