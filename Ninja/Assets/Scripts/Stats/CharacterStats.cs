using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour, IHurtboxResponder
{   
    [Header("Name")]
    [SerializeField] public string charName;

    [Header("Faction")]
    [SerializeField] private Faction faction;
    public string Faction {get{return faction.FactionName;}}
    
    [Header("Hurtboxes")]
    [SerializeField] private Hurtbox[] hurtboxes;
    
    [Header("Attributes")]
    [SerializeField] private Attribute strength;
    [SerializeField] private Attribute agility;
    [SerializeField] private Attribute dexterity;
    [SerializeField] private Attribute wisdom;
    
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
    public event deathDelegate OnDeath;

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

    #region Modifiers

    private Dictionary<Modifier, Attribute> modifiers = new Dictionary<Modifier, Attribute>();

    private void AddModifier(Modifier modifier)
    {
        if(modifiers.ContainsKey(modifier)) return;

        Attribute attr = FindAttribute(modifier.Attribute);
        if(attr == null) return;

        // Add modifier to attribute
        attr.AddModifier(modifier);

        // Add to dictionary
        modifiers.Add(modifier, attr);
    }

    private void RemoveModifier(Modifier modifier)
    {
        if(modifiers.ContainsKey(modifier) == false) return;
        
        // Remove modifier from attribute
        modifiers[modifier].RemoveModifier(modifier);

        // Remove from dicitionary
        modifiers.Remove(modifier);
    }

    private void AddModifiers(List<Modifier> modifiers)
    {
        for (int i = 0; i < modifiers.Count; i++)
            AddModifier(modifiers[i]);
    }

    private void RemoveModifiers(List<Modifier> modifiers)
    {
        for (int i = 0; i < modifiers.Count; i++)
            RemoveModifier(modifiers[i]);
    }


    #endregion

    #region Traits

    [SerializeField] private List<string> boons = new List<string>();
    [SerializeField] private List<string> flaws = new List<string>();

    public void AddBoon(string boonName)
    {
        if(boons.Contains(boonName) == false)
            boons.Add(boonName);

        // Add modifiers
        Trait t = TraitManager.instance.GetBoon(boonName);
        AddModifiers(t.modifiers);
    }

    public void RemoveBoon(string boonName)
    {
        if(boons.Contains(boonName))
            boons.Remove(boonName);

        // Remove modifiers
        Trait t = TraitManager.instance.GetBoon(boonName);
        RemoveModifiers(t.modifiers);
    }

    public void AddFlaw(string flawName)
    {
        if(flaws.Contains(flawName) == false)
            flaws.Add(flawName);
        
        // Add modifiers
        Trait t = TraitManager.instance.GetFlaw(flawName);
        AddModifiers(t.modifiers);
    }

    public void RemoveFlaw(string flawName)
    {
        if(flaws.Contains(flawName))
            flaws.Remove(flawName);

        // Add modifiers
        Trait t = TraitManager.instance.GetFlaw(flawName);
        RemoveModifiers(t.modifiers);
    }

    
    #endregion

    [SerializeField] private bool guardOn;    
    [SerializeField] private Direction9 guardDirection;

    public void SetGuard(bool isGuarding, Direction9 direction = Direction9.CENTER)
    {
        guardOn = isGuarding;
        guardDirection = direction;    
    }

    #endregion

    #region Sit

    private bool isSit;
    [SerializeField] private float sitHealthRecoveryMult = 1.5f;
    [SerializeField] private float sitStaminaRecoveryMult = 2f;

    #endregion

    public delegate void HitByDelegate(StealthAgent agent);
    public event HitByDelegate OnHitBy;

    public delegate void HitDelegate(int softDamage, int hardDamage);
    public event HitDelegate OnHit;

    [SerializeField] private ChakraManager chakraManager;

    private void Awake() 
    {
        InitializeAttributes();
    }

    void Start()
    {
        foreach(Hurtbox h in hurtboxes)
            h.AddResponder(this);
        
        Health = MaxHealth;
        PotentialHealth = MaxHealth;
        Stamina = MaxStamina;

        if(gameObject.name == "Player Object (Main)")
        {
            DebugCommandDatabase.AddCommand(new DebugCommand(
                "setattr", 
                "Sets attribute to desired value", 
                "setattribute <attribute> <level>", 
                (string[] parameters) => {
                    AttributeType attrType;
                    if(Enum.TryParse(parameters[0], true, out attrType))
                    {
                        bool success = SetAttributeLevel(attrType, Int32.Parse(parameters[1]));
                        if(success)
                            return "Set " + parameters[0] + ": " + parameters[1];
                    }
                    return "Unknown attribute: " + (parameters[0]);
                }));

            DebugCommandDatabase.AddCommand(new DebugCommand(
                "set", 
                "Sets attribute to desired value", 
                "set <attribute> <level>", 
                (string[] parameters) => {
                    bool success = false;

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
                        if(parameters[0] == "str") success = SetAttributeLevel(AttributeType.STRENGTH, Int32.Parse(parameters[1]));
                        else if(parameters[0] == "agi") success = SetAttributeLevel(AttributeType.AGILITY, Int32.Parse(parameters[1]));
                        else if(parameters[0] == "dex") success = SetAttributeLevel(AttributeType.DEXTERITY, Int32.Parse(parameters[1]));
                        else if(parameters[0] == "wis") success = SetAttributeLevel(AttributeType.WISDOM, Int32.Parse(parameters[1]));
                        else
                        {
                            AttributeType attrType;
                            if(Enum.TryParse(parameters[0], true, out attrType))
                                success = SetAttributeLevel(attrType, Int32.Parse(parameters[1]));

                        }
                        break;
                    }
                    
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

    private void InitializeAttributes()
    {
        strength.Initialize(1, 10);
        agility.Initialize(1, 10);
        dexterity.Initialize(1, 10);
        wisdom.Initialize(1, 10);
    }

    public Attribute FindAttribute(AttributeType attributeType)
    {
        switch(attributeType)
        {
            case AttributeType.STRENGTH:
                return strength;
            case AttributeType.AGILITY:
                return agility;
            case AttributeType.DEXTERITY:
                return dexterity;
            case AttributeType.WISDOM:
                return wisdom;
        }

        return null;
    }
    
    public int GetAttributeLevel(AttributeType attributeType)
    {
        Attribute attr = FindAttribute(attributeType);
        if (attr != null) 
            return attr.Level;

        return -1;
    }

    public int GetAttributeLevelModified(AttributeType attributeType)
    {
        Attribute attr = FindAttribute(attributeType);
        if (attr != null) 
            return attr.Modified;

        return -1;
    }

    public float GetAttributeExp(AttributeType attributeType)
    {
        Attribute attr = FindAttribute(attributeType);
        if (attr != null) 
            return attr.Experience;

        return -1f;
    }

    public bool IncreaseAttribute(AttributeType attributeType)
    {
        Attribute s = FindAttribute(attributeType);
        if (s != null) 
            return s.IncreaseLevel();
            
        return false;
    }

    public bool IncreaseAttributeExp(AttributeType attributeType, float amount)
    {
        Attribute s = FindAttribute(attributeType);
        if (s != null) 
            return s.GainExperience(amount);
            
        return false;
    }

    public bool SetAttributeLevel(AttributeType attributeType, int level)
    {
        Attribute s = FindAttribute(attributeType);
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
        
        float healthMult = (isSit)? sitHealthRecoveryMult : 1;
        float staminaMult = (isSit)? sitStaminaRecoveryMult : 1;

        if(Time.time - _healthLastDeplete > _healthRecoveryStart) 
            AddHealth(_healthRecovery * healthMult * Time.deltaTime);

        if(Time.time - _potentialHealthLastDeplete > _potentialHealthRecoveryStart) 
            AddPotentialHealth(_potentialHealthRecovery * healthMult * Time.deltaTime);

        if(Time.time - _staminaLastDeplete > _staminaRecoveryStart) 
            AddStamina(_staminaRecovery * staminaMult * Time.deltaTime);

        if(Time.time - _potentialStaminaLastDeplete > _potentialStaminaRecoveryStart) 
            AddPotentialStamina(_potentialStaminaRecovery * staminaMult * Time.deltaTime);
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

    public bool DepleteStamina(float amount, bool greedy = false)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to deplete a negative amount of Stamina!");
            return false;
        }

        if(Stamina < amount)
        {
            if(greedy)
                SubStamina(amount);
            return false;
        }

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

    public bool GetHit(StealthAgent agent, int softDamage, int hardDamage, DamageType damageType, Direction9 direction)
    {
        // Send hit events - Can be listened to by AI, AnimationManager, etc.
        if(OnHitBy != null)
            OnHitBy(agent);
        
        if(OnHit != null)
            OnHit(softDamage, hardDamage);
        
        // Check if guarding in right direction
        if(guardOn)
        {
            if( direction == Direction9.CENTER ||
                direction == Direction9.UP && guardDirection == Direction9.UP ||
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

        if(OnDeath != null)
            OnDeath();
    }

    public bool CanPickup(WeightClass weightClass)
    {
        switch(weightClass)
        {
            case WeightClass.LIGHT:
                return true;
            case WeightClass.MEDIUM:
                return GetAttributeLevel(AttributeType.STRENGTH) >= 5;
            case WeightClass.HEAVY:
                return GetAttributeLevel(AttributeType.STRENGTH) >= 8;
        }

        return false;
    }
    
    public void SetSit(bool state)
    {
        isSit = state;
    }

    public bool DepleteChakra(ChakraType type, float amount)
    {
        return chakraManager.DepleteChakra(type, amount);
    }

    public void ChargeChakra(ChakraType type, float amount)
    {
        chakraManager.ChargeChakra(type, amount);
    }

    public void Convert(ChakraType from, ChakraType to, float amount)
    {
        chakraManager.Convert(from, to, amount, 1f);
    }

    public void Focus(ChakraType type)
    {
        chakraManager.Focus(type);
    }

    public float GetChakraAmount(ChakraType type)
    {
        return chakraManager.GetChakraAmount(type);
    }

    public float GetRelationship(string factionName)
    {
        if(faction == null)
            return 0;
        return faction.GetRelationship(factionName);
    }
}
