using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour, IHurtboxResponder
{
    [SerializeField] private Hurtbox[] hurtboxes;
    
    [Header("Attributes")]
    [SerializeField] private string[] attributeNames = {"Strength", "Endurance", "Agility", "Dexterity", "Mind"};
    private int oldAttributeLength = 0;
    [SerializeField] private Attribute[] attributes;
    
    [SerializeField] private const int minAttributeLevel = 1;
    [SerializeField] private const int maxAttributeLevel = 10;

    [Header("Health")]
    [SerializeField] private bool isAlive = true;
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private string _maxHealthStatModifier = "Endurance";
    [SerializeField] private int[] _maxHealthPerStat = {70, 80, 90, 100 ,115 ,130, 150, 170, 200, 230};
    [SerializeField] private float _health = 100;
    public float maxHealth {get { return _maxHealthPerStat[GetAttributeLevel(_maxHealthStatModifier)]; }}
    public float health 
    {
        get { return _health; } 
        private set 
        { 
            float oldValue = _health;
            _health = Mathf.Clamp(value, 0, maxHealth); 
            
            if (HealthUpdateEvent != null)
                HealthUpdateEvent(_health - oldValue); 

            if(_health <= 0)
                Die();
        }
    }

    public delegate void healthUpdateDelegate(float delta);
    public event healthUpdateDelegate HealthUpdateEvent;

    public delegate void deathDelegate();
    public event deathDelegate DeathEvent;

    [Tooltip("Health recovery per second")]
    [SerializeField] private float _healthRecovery = .5f;
    [SerializeField] private float _healthRecoveryStart = 20f;
    [SerializeField] private float _healthLastDeplete;

    [Header("Stamina")]
    [SerializeField] private float _maxStamina = 100;
    [SerializeField] private float _stamina = 100;
    public float maxStamina {get { return _maxStamina; }}
    public float stamina 
    {
        get { return _stamina; }  
        private set 
        { 
            float oldValue = _stamina;
            _stamina = Mathf.Clamp(value, 0, maxStamina); 
            
            if (StaminaUpdateEvent != null)
                StaminaUpdateEvent(_stamina - oldValue); 
        }
    }
        
    public delegate void staminaUpdateDelegate(float delta);
    public event staminaUpdateDelegate StaminaUpdateEvent;

    [Tooltip("Stamina recovery per second")]
    [SerializeField] private float _staminaRecovery = .5f;
    [SerializeField] private float _staminaRecoveryStart = 1f;
    [SerializeField] private float _staminaLastDeplete;

    void Start()
    {
        foreach(Hurtbox h in hurtboxes)
            h.AddResponder(this);
    }

    public void OnValidate()
    {
        if(oldAttributeLength != attributeNames.Length)
            InitializeAttributes();
    }

    void InitializeAttributes()
    {
        attributes = new Attribute[attributeNames.Length];

        for (int i = 0; i < attributeNames.Length; i++)
            attributes[i] = new Attribute(attributeNames[i], minAttributeLevel, maxAttributeLevel);

        oldAttributeLength = attributeNames.Length;
    }

    Attribute FindAttribute(string attributeName)
    {
        attributeName = char.ToUpper(attributeName[0]) + attributeName.Substring(1).ToLower();
        for (int i = 0; i < attributes.Length; i++)
            if(attributes[i]._name == attributeName)
                return attributes[i];

        return null;
    }
    
    public int GetAttributeLevel(string attributeName)
    {
        Attribute attr = FindAttribute(attributeName);
        if (attr != null) 
            return attr._level;

        return -1;
    }

    public float GetAttributeExp(string attributeName)
    {
        Attribute attr = FindAttribute(attributeName);
        if (attr != null) 
            return attr._experience;

        return -1f;
    }

    public bool IncreaseAttribute(string statName)
    {
        Attribute s = FindAttribute(statName);
        if (s != null) 
            return s.IncreaseLevel();
            
        return false;
    }

    public bool IncreaseAttributeExp(string statName, float amount)
    {
        Attribute s = FindAttribute(statName);
        if (s != null) 
            return s.GainExperience(amount);
            
        return false;
    }

    void Update()
    {
        RegenUpdate();
    }

    void RegenUpdate()
    {
        if(Time.time - _healthLastDeplete > _healthRecoveryStart) 
            AddHealth(_healthRecovery * Time.deltaTime);
        if(Time.time - _staminaLastDeplete > _staminaRecoveryStart) 
            AddStamina(_staminaRecovery * Time.deltaTime);
    }

    #region Health Change

    public void AddHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of health!");
            return;
        }
            
        health += amount;
    }

    public void SubHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of health!");
            return;
        }
            
        health -= amount;
        _healthLastDeplete = Time.time;
    }

    #endregion
    #region Stamina Change

    public void AddStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of stamina!");
            return;
        }
            
        stamina += amount;
    }

    public void SubStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of stamina!");
            return;
        }
            
        stamina -= amount;
        _staminaLastDeplete = Time.time;
    }

    public bool DepleteStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of stamina!");
            return false;
        }

        if(stamina < amount)
            return false;

        SubStamina(amount);
        return true;
    }

    #endregion

    public void GetHit(int damage)
    {
        Debug.Log("I was hit for " + damage + " damage");
        SubHealth(damage);
    }

    public void Die()
    {
        isAlive = false;
        if(DeathEvent != null)
            DeathEvent();
    }

    public bool CanPickup(WeightClass weightClass)
    {
        switch(weightClass)
        {
            case WeightClass.LIGHT:
                return true;
            case WeightClass.MEDIUM:
                return GetAttributeLevel("strength") >= 5;
            case WeightClass.HEAVY:
                return GetAttributeLevel("strength") >= 8;
        }

        return false;
    }
    
}
