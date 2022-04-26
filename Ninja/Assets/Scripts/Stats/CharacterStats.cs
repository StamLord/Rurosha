using System;
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

    #region Health

    [Header("Health")]
    [SerializeField] private bool _isAlive = true;
    public bool IsAlive {get {return _isAlive;}}

    [SerializeField] private GameObject aliveVisual;
    [SerializeField] private GameObject deadVisual;

    [SerializeField] private AttributeDependant<int> _maxHealth;
    public int MaxHealth {get { return _maxHealth.GetValue(this); }}

    [SerializeField] private float _health = 100;
    [SerializeField] private float _potentialHealth = 100;

    public float Health 
    {
        get { return _health; } 
        private set 
        { 
            float oldValue = _health;
            _health = Mathf.Clamp(value, 0, PotentialHealth); 
            
            if (HealthUpdateEvent != null && oldValue != _health)
                HealthUpdateEvent(_health / MaxHealth); 

            if(_health <= 0)
                Die();
        }
    }

    public float PotentialHealth
    {
        get { return _potentialHealth; }
        private set 
        {
            float oldValue = _potentialHealth;
            _potentialHealth = Mathf.Clamp(value, 0, MaxHealth); 
            
            if (PotentialHealthUpdateEvent != null && oldValue != _potentialHealth)
                PotentialHealthUpdateEvent(_potentialHealth / MaxHealth); 

            if(_potentialHealth < _health)
                Health = _potentialHealth;
        }
    }

    public delegate void healthUpdateDelegate(float health);
    public event healthUpdateDelegate HealthUpdateEvent;

    public delegate void potentialHealthUpdateDelegate(float potentialHealth);
    public event potentialHealthUpdateDelegate PotentialHealthUpdateEvent;

    public delegate void deathDelegate();
    public event deathDelegate DeathEvent;

    [Tooltip("Health recovery per second")]

    [SerializeField] private AttributeDependant<float> HealthRecovery;
    private float _healthRecovery {get {return HealthRecovery.GetValue(this);}}
    [SerializeField] private float _healthRecoveryStart = 20f;
    [SerializeField] private float _healthLastDeplete;

    [SerializeField] private AttributeDependant<float> PotentialHealthRecovery;
    private float _potentialHealthRecovery {get {return PotentialHealthRecovery.GetValue(this);}}
    [SerializeField] private float _potentialHealthRecoveryStart = 60f;
    [SerializeField] private float _potentialHealthLastDeplete;

    #endregion

    #region Stamina

    [Header("Stamina")]
    [SerializeField] private float _stamina = 100;
    [SerializeField] private float _potentialStamina = 100;

    [SerializeField] private AttributeDependant<int> _maxStamina;
    public int MaxStamina {get { return _maxStamina.GetValue(this); }}
    
    public float Stamina 
    {
        get { return _stamina; }  
        private set 
        { 
            float oldValue = _stamina;
            _stamina = Mathf.Clamp(value, 0, PotentialStamina); 
            
            if (StaminaUpdateEvent != null && oldValue != _stamina)
                StaminaUpdateEvent(_stamina / MaxStamina); 
        }
    }

    public float PotentialStamina 
    {
        get { return _potentialStamina; }  
        private set 
        { 
            float oldValue = _potentialStamina;
            _potentialStamina = Mathf.Clamp(value, 0, MaxStamina); 
            
            if (PotentialStaminaUpdateEvent != null && oldValue != _potentialStamina)
                PotentialStaminaUpdateEvent(_potentialStamina / MaxStamina); 

            if(_potentialStamina < _stamina)
                Stamina = _potentialStamina;
        }
    }
        
    public delegate void staminaUpdateDelegate(float stamina);
    public event staminaUpdateDelegate StaminaUpdateEvent;

    public delegate void potentialStaminaUpdateDelegate(float potentialStamina);
    public event potentialStaminaUpdateDelegate PotentialStaminaUpdateEvent;

    [Tooltip("Stamina recovery per second")]

    [SerializeField] private AttributeDependant<float> StaminaRecovery;
    private float _staminaRecovery {get {return StaminaRecovery.GetValue(this);}}
    [SerializeField] private float _staminaRecoveryStart = 1f;
    [SerializeField] private float _staminaLastDeplete;

    [SerializeField] private AttributeDependant<float> PotentialStaminaRecovery;
    private float _potentialStaminaRecovery {get {return PotentialStaminaRecovery.GetValue(this);}}
    [SerializeField] private float _potentialStaminaRecoveryStart = 10f;
    [SerializeField] private float _potentialStaminaLastDeplete;

    #endregion

    #region Guard

    [SerializeField] private bool guardOn;    
    [SerializeField] private Direction9 guardDirection;

    public void SetGuard(bool isGuarding, Direction9 direction = Direction9.CENTER)
    {
        guardOn = isGuarding;
        guardDirection = direction;    
    }

    #endregion

    void Start()
    {
        foreach(Hurtbox h in hurtboxes)
            h.AddResponder(this);
        
        Health = MaxHealth;
        PotentialHealth = MaxHealth;
        Stamina = MaxStamina;

        //InitializeAttributes();

        if(gameObject.name == "Player Object (Main)")
        {
            DebugCommandDatabase.AddCommand(new DebugCommand(
                "setattribute", 
                "Sets attribute to desired value", 
                "setattribute <attribute> <level>", 
                (string[] parameters) => {
                    bool success = SetAttributeLevel(parameters[0], Int32.Parse(parameters[1]));
                    if(success)
                        return "Set " + parameters[0] + ": " + parameters[1];
                    return "Unknown attribute: " + (parameters[0]);
                }));

            DebugCommandDatabase.AddCommand(new DebugCommand(
                "set", 
                "Sets attribute to desired value", 
                "set <attribute> <level>", 
                (string[] parameters) => {
                    bool success;

                    switch(parameters[0])
                    {
                    case "hp":
                        Health = Int32.Parse(parameters[1]);
                        success = true;
                        break;
                    case "php":
                        PotentialHealth = Int32.Parse(parameters[1]);
                        success = true;
                        break;
                    case "st":
                        Stamina = Int32.Parse(parameters[1]);
                        success = true;
                        break;
                    case "pst":
                        PotentialStamina = Int32.Parse(parameters[1]);
                        success = true;
                        break;
                    default:
                        // Parse short attribute names
                        string par = parameters[0];
                        if(par == "str") par = "strength";
                        if(par == "agi") par = "agility";
                        if(par == "dex") par = "dexterity";
                        if(par == "min" || par == "mnd") par = "mind";
                        success = SetAttributeLevel(par, Int32.Parse(parameters[1]));
                        break;
                    }
                    
                    if(success)
                        return "Set " + parameters[0] + ": " + parameters[1];
                    return "Unknown attribute: " + (parameters[0]);
                }));

            DebugCommandDatabase.AddCommand(new DebugCommand(
                "setattr", 
                "Sets attribute to desired value", 
                "setattr <attribute> <level>", 
                (string[] parameters) => {
                    bool success = SetAttributeLevel(parameters[0], Int32.Parse(parameters[1]));
                    if(success)
                        return "Set " + parameters[0] + ": " + parameters[1];
                    return "Unknown attribute: " + (parameters[0]);
                }));

            DebugCommandDatabase.AddCommand(new DebugCommand(
                "killme", 
                "Kills the player", 
                "killme", 
                (string[] parameters) => {
                    SubHealth(9999f);
                    return "Killed player";
                }));
        }
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

    public Attribute FindAttribute(string attributeName)
    {
        if(attributeName.Length == 0)
        {
            Debug.LogWarning("Attribute name to search cannot be empty!", gameObject);
            return null;
        }

        attributeName = char.ToUpper(attributeName[0]) + attributeName.Substring(1).ToLower();
        for (int i = 0; i < attributes.Length; i++)
            if(attributes[i].Name == attributeName)
                return attributes[i];

        return null;
    }
    
    public int GetAttributeLevel(string attributeName)
    {
        Attribute attr = FindAttribute(attributeName);
        if (attr != null) 
            return attr.Level;

        return -1;
    }

    public float GetAttributeExp(string attributeName)
    {
        Attribute attr = FindAttribute(attributeName);
        if (attr != null) 
            return attr.Experience;

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

    public bool SetAttributeLevel(string statName, int level)
    {
        Attribute s = FindAttribute(statName);
        if (s != null) 
        {   
            // s.Level = Mathf.Clamp(level, minAttributeLevel, maxAttributeLevel);
            s.SetLevel(level);
            return true;
        }
            
        return false;
    }

    void Update()
    {
        RegenUpdate();
    }

    void RegenUpdate()
    {
        if(_isAlive == false) return;
        
        if(Time.time - _healthLastDeplete > _healthRecoveryStart) 
            AddHealth(_healthRecovery * Time.deltaTime);

        if(Time.time - _potentialHealthLastDeplete > _potentialHealthRecoveryStart) 
            AddPotentialHealth(_potentialHealthRecovery * Time.deltaTime);

        if(Time.time - _staminaLastDeplete > _staminaRecoveryStart) 
            AddStamina(_staminaRecovery * Time.deltaTime);

        if(Time.time - _potentialStaminaLastDeplete > _potentialStaminaRecoveryStart) 
            AddPotentialStamina(_potentialStaminaRecovery * Time.deltaTime);
    }

    #region Health Change

    public void AddHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of Health!");
            return;
        }
            
        Health += amount;
    }

    public void SubHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of Health!");
            return;
        }
            
        Health -= amount;
        _healthLastDeplete = Time.time;
    }

    public void AddPotentialHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of potential Health!");
            return;
        }
            
        PotentialHealth += amount;
    }

    public void SubPotentialHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of potential Health!");
            return;
        }
            
        PotentialHealth -= amount;
        _potentialHealthLastDeplete = Time.time;
    }

    #endregion
    #region Stamina Change

    public void AddStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of Stamina!");
            return;
        }
            
        Stamina += amount;
    }

    public void SubStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of Stamina!");
            return;
        }
            
        Stamina -= amount;
        _staminaLastDeplete = Time.time;
    }

    public bool DepleteStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to deplete a negative amount of Stamina!");
            return false;
        }

        if(Stamina < amount)
            return false;

        SubStamina(amount);
        return true;
    }

    public void AddPotentialStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of potential Stamina!");
            return;
        }
            
        PotentialStamina += amount;
    }

    public void SubPotentialStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of potential Stamina!");
            return;
        }
            
        PotentialStamina -= amount;
        _potentialStaminaLastDeplete = Time.time;
    }

    public bool DepletePotentailStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to deplete a negative amount of potential Stamina!");
            return false;
        }

        if(PotentialStamina < amount)
            return false;

        SubPotentialStamina(amount);
        return true;
    }

    #endregion

    public bool GetHit(int softDamage, int hardDamage, DamageType damageType, Direction9 direction)
    {
        // Check if guarding in right direction
        if(guardOn)
        {
            if(direction == Direction9.UP && guardDirection == Direction9.UP ||
                direction == Direction9.DOWN && guardDirection == Direction9.DOWN ||
                direction == Direction9.LEFT && guardDirection == Direction9.RIGHT ||
                direction == Direction9.RIGHT && guardDirection == Direction9.LEFT ||
                direction == Direction9.DOWNRIGHT && guardDirection == Direction9.DOWNLEFT ||
                direction == Direction9.DOWNLEFT && guardDirection == Direction9.DOWNRIGHT ||
                direction == Direction9.UPLEFT && guardDirection == Direction9.UPRIGHT ||
                direction == Direction9.UPRIGHT && guardDirection == Direction9.UPLEFT)
                return false;
        }
        
        Debug.Log(gameObject.name + " was hit for " + softDamage + " / " + hardDamage + " " + damageType + " damage");
        SubHealth(softDamage);
        SubPotentialHealth(hardDamage);
        return true;
    }

    public void Die()
    {
        _isAlive = false;

        if(aliveVisual) aliveVisual.SetActive(false);
        
        if(deadVisual)
        { 
            deadVisual.SetActive(true);
            // Allign dead object
            deadVisual.transform.position = aliveVisual.transform.position;
            deadVisual.transform.rotation = aliveVisual.transform.rotation;
        }

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
