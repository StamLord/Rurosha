using System;
using System.Linq;
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
    
    #region Exp Drop
    
    [Header("Exp Drop")]
    [SerializeField] private int expDrop;
    public int ExpDrop {get{return expDrop;}}

    #endregion

    #region Level

    [Header("Level")]

    [Min(1)]
    [SerializeField] private int level = 1;
    public int Level {get{return level;}}

    [SerializeField] private int experience;
    public int Experience {get{return experience;}}

    [SerializeField] private int[] expNeeded;
    public int NextLevel {
        get{
            // Return last value if we our level is too high.
            if(level >= expNeeded.Length)
                return expNeeded[expNeeded.Length - 1];
            // Return first value if for some reason we are below level 1.
            else if(level < 1)
                return expNeeded[0];
            // Return exp needed for level up
            else
                return expNeeded[level - 1];
            }}

    public bool AddExp(int amount)
    {
        experience += amount;

        // Add exp event
        if(OnAddExp != null)
            OnAddExp(amount);

        // If we're outside of expNeeded[] range we use it's last value
        int expGoal = NextLevel;
        
        // If beyond needed exp, we level up!
        if(experience >= expGoal)
        {
            experience -= expGoal;
            level++;
            if(skillTreeManager)
                skillTreeManager.AddSkillPoint(skillPointsPerLevel);

            // Level up event
            if(OnLevelUp != null)
                OnLevelUp(level);
            
            return true;
        }

        return false;
    }

    public delegate void AddExpDelegate(int amount);
    public event AddExpDelegate OnAddExp;

    public delegate void LevelUpDelegate(int level);
    public event LevelUpDelegate OnLevelUp;

    #endregion

    #region Attributes

    [Header("Attributes")]
    [SerializeField] private Attribute strength;
    [SerializeField] private Attribute agility;
    [SerializeField] private Attribute dexterity;
    [SerializeField] private Attribute wisdom;
    
    [SerializeField] private const int minAttributeLevel = 1;
    [SerializeField] private const int maxAttributeLevel = 10;

    #endregion

    #region Status Manager

    [Header("Status Manager")]
    [SerializeField] private StatusManager statusManager;

    #endregion

    #region Karma
    
    [Header("Karma")]
    [SerializeField][Range(-100,100)] private int karma;
    public int Karma {
        get {return karma;} 
        private set {
            karma = Mathf.Clamp(karma + value, -100, 100);
            
            // Karma event
            if(OnKarmaUpdate != null)
                OnKarmaUpdate(value);
            }}

    [SerializeField] private int stealKarma = -5;
    public void CommitSteal()
    {
        Karma += stealKarma;
    }

    public void AddKarma(int karma)
    {
        Karma += karma;
    }

    public delegate void OnKarmaUpdateDelegate(int amount);
    public event OnKarmaUpdateDelegate OnKarmaUpdate;

    #endregion

    #region Skills

    [Header("Skill Tree Manager")]
    [SerializeField] private SkillTreeManager skillTreeManager;
    [SerializeField] private int skillPointsPerLevel = 1;

    public bool IsSkillLearned(string skillName)
    {
        return skillTreeManager.IsLearned(skillName);
    }

    #endregion

    #region Money

    [Header("Money")]
    [SerializeField] private int money;

    public int Money {
        private set { money = Mathf.Max(value, 0); }
        get { return money; }
        }
    
    public delegate void MoneyUpdateDelegate(int change);
    public event MoneyUpdateDelegate OnMoneyUpdate;

    public void AddMoney(int amount)
    {
        Money += amount;

        if(OnMoneyUpdate != null)
            OnMoneyUpdate(amount);
    }

    public bool DepleteMoney(int amount, bool greedy = false)
    {
        // If enough money, reduce the amount
        if(amount <= Money)
        {
            Money -= amount;
            
            if(OnMoneyUpdate != null)
                OnMoneyUpdate(-amount);
            
            return true;
        }
        // If not enough money, but greedy, reduce the existing amount
        else if (greedy)
        {
            if(OnMoneyUpdate != null)
                OnMoneyUpdate(Money);
            Money = 0;
        }
        
        return false;
    }

    #endregion

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
            
            if(_health <= 0 && IsAlive)
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

    public delegate void healthRestoreDelegate(float health, float potentialHealth);
    public event healthRestoreDelegate OnHealthRestore;

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

    public delegate void staminaRestoreDelegate(float stamina, float potentialStamina);
    public event staminaRestoreDelegate OnStaminaRestore;

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

    #region Modifiers

    private Dictionary<Modifier, Modifieable> modifiers = new Dictionary<Modifier, Modifieable>();

    // Modifier : Timestamp pairs
    private Dictionary<Modifier, float> tempModifiers = new Dictionary<Modifier, float>();

    public void AddModifier(AttributeModifier attrModifier)
    {
        Modifier modifier = attrModifier.modifier;

        if(modifiers.ContainsKey(modifier))
        {
            // Reset timestamp
            if(modifier.IsTemporary)
                tempModifiers[modifier] = Time.time;
            return;
        }

        // Get attribute
        Attribute attr = FindAttribute(attrModifier.attribute);
        if(attr == null) return;

        // Add modifier to attribute
        attr.AddModifier(modifier);

        // Add to dictionary
        modifiers.Add(modifier, attr);

        // Add to temporary dictionary with timestamp
        if(modifier.IsTemporary)
            tempModifiers[modifier] = Time.time;
    }

    public void AddModifier(ElementResistanceModifier elementResistanceModifier)
    {
        elementalResistanceMatrix.AddModifier(elementResistanceModifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        if(modifiers.ContainsKey(modifier))
        {
            // Remove modifier from attribute
            modifiers[modifier].RemoveModifier(modifier);

            // Remove from dicitionary
            modifiers.Remove(modifier);
        }

        if(tempModifiers.ContainsKey(modifier))
            tempModifiers.Remove(modifier);
    }

    public void AddModifiers(List<AttributeModifier> attrModifiers)
    {
        for (int i = 0; i < attrModifiers.Count; i++)
            AddModifier(attrModifiers[i]);
    }

    public void AddModifiers(List<ElementResistanceModifier> elementResistanceModifier)
    {
        elementalResistanceMatrix.AddModifiers(elementResistanceModifier);
    }

    public void RemoveModifiers(List<Modifier> modifiers)
    {
        for (int i = 0; i < modifiers.Count; i++)
            RemoveModifier(modifiers[i]);
    }

    private void CheckOverdueModifiers()
    {
        // Make a list of overdue modifiers
        List<Modifier> toRemove = new List<Modifier>();
        
        foreach(var mod in tempModifiers)
        {
            Modifier modifier = mod.Key;
            if(Time.time - tempModifiers[modifier] >= modifier.Duration)
                toRemove.Add(modifier);
        }

        // Remove modifiers
        RemoveModifiers(toRemove);
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
        AddModifiers(t.attributeModifiers);
        AddModifiers(t.elementalResistanceModifiers);
    }

    public void RemoveBoon(string boonName)
    {
        if(boons.Contains(boonName))
            boons.Remove(boonName);

        // Remove modifiers
        Trait t = TraitManager.instance.GetBoon(boonName);
        RemoveModifiers(t.AllModifiers);
    }

    public void AddFlaw(string flawName)
    {
        if(flaws.Contains(flawName) == false)
            flaws.Add(flawName);
        
        // Add modifiers
        Trait t = TraitManager.instance.GetFlaw(flawName);
        AddModifiers(t.attributeModifiers);
    }

    public void RemoveFlaw(string flawName)
    {
        if(flaws.Contains(flawName))
            flaws.Remove(flawName);

        // Add modifiers
        Trait t = TraitManager.instance.GetFlaw(flawName);
        RemoveModifiers(t.AllModifiers);
    }

    
    #endregion

    #region Sit

    private bool isSit;
    [SerializeField] private float sitHealthRecoveryMult = 1.5f;
    [SerializeField] private float sitStaminaRecoveryMult = 2f;

    #endregion

    #region Heat Damage
    
    [SerializeField] private float minHeatDamageTemperature = 40;
    [SerializeField] private float HeatDamageCooldown = 1f;
    [SerializeField] private float heatSoftDamage = 10;
    [SerializeField] private float heatHardDamage = 0;
    [SerializeField] private Status heatStatus;

    private float lastHeatDamage;

    #endregion

    #region Damage Resistance
    
    [SerializeField] private DamageResistanceMatrix damageResistanceMatrix;

    #endregion

    #region Elemental Resistance
    
    [SerializeField] private ElementalResistanceMatrix elementalResistanceMatrix;

    #endregion

    public delegate void HitByDelegate(StealthAgent agent);
    public event HitByDelegate OnHitBy;

    public delegate void HitDelegate(int softDamage, int hardDamage);
    public event HitDelegate OnHit;

    public delegate void ForceDelegate(Vector3 force);
    public event ForceDelegate OnForce;

    [SerializeField] private ChakraManager chakraManager;

    private void Awake() 
    {
        InitializeAttributes();
    }

    void Start()
    {
        // Subscribe to status manager updates
        if(statusManager) 
        {
            statusManager.OnStatusUpdate += StatusUpdate;
            statusManager.OnStatusStart += StatusStart;
            statusManager.OnStatusEnd += StatusEnd;
        }

        // Setup hurtboxes
        foreach(Hurtbox h in hurtboxes)
            h.AddResponder(this);
        
        // Initialize Stats
        Health = MaxHealth;
        PotentialHealth = MaxHealth;
        Stamina = MaxStamina;
        PotentialStamina = MaxStamina;

        // Only add these commands if we are the player
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
                        case "karma":
                            Karma = Int32.Parse(parameters[1]);
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
            
            DebugCommandDatabase.AddCommand(new DebugCommand(
                "addskillpoint", 
                "Adds skill points", 
                "addskillpoint <amount>", 
                (string[] parameters) => {
                    
                    int amount;
                    if(Int32.TryParse(parameters[0], out amount) == false)
                        amount = 1;
                    
                    skillTreeManager.AddSkillPoint(amount);
                    return "Added " + amount + " Skill Points";
                }));
            
            DebugCommandDatabase.AddCommand(new DebugCommand(
                "rosebud", 
                "Gives 1000 coins to the player.", 
                "rosebud", 
                (string[] parameters) => {
                    AddMoney(1000);
                    return "Added 1000 coins.";
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

    private void Update()
    {
        RegenUpdate();
        CheckOverdueModifiers();
    }

    private void RegenUpdate()
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

    private void AddHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of Health!");
            return;
        }

        if(amount == 0)
            return;
            
        Health += amount;
    }

    private void SubHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of Health!");
            return;
        }
        
        if(amount == 0)
            return;
        
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

        if(amount == 0)
            return;
            
        PotentialHealth += amount;
    }

    public void SubPotentialHealth(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of potential Health!");
            return;
        }
        
        if(amount == 0)
            return;
        
        PotentialHealth -= amount;
        _potentialHealthLastDeplete = Time.time;
    }

    /// <summary>
    /// Called by items, spells and outside effects to restore health immediately.
    /// Calls OnHealthRestore event.
    /// </summary>
    public void RestoreHealth(float health, float potentialHealth)
    {
        AddHealth(health);
        AddPotentialHealth(potentialHealth);

        if(OnHealthRestore != null)
            OnHealthRestore(health, potentialHealth);
    }

     /// <summary>
    /// Called by items, spells and outside effects to deplete health immediately.
    /// Calls OnHit event.
    /// </summary>
    public void DepleteHealth(int health, int potentialHealth)
    {
        SubHealth(health);
        SubPotentialHealth(potentialHealth);

        if(OnHit != null)
            OnHit(health, potentialHealth);
    }

    #endregion
    #region Stamina Change

    private void AddStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of Stamina!");
            return;
        }
        
        if(amount == 0)
            return;
        
        Stamina += amount;
    }

    private void SubStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of Stamina!");
            return;
        }
        
        if(amount == 0)
            return;
        
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

    private void AddPotentialStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to add a negative amount of potential Stamina!");
            return;
        }

        if(amount == 0)
            return;
            
        PotentialStamina += amount;
    }

    private void SubPotentialStamina(float amount)
    {
        if(amount < 0)
        {
            Debug.LogWarning("You are trying to subtract a negative amount of potential Stamina!");
            return;
        }
        
        if(amount == 0)
            return;
        
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

    /// <summary>
    /// Called by items, spells and outside effects to restore stamina immediately
    /// Calls OnStaminaRestore event.
    /// </summary>
    public void RestoreStamina(float stamina, float potentialStamina)
    {
        AddStamina(stamina);
        AddPotentialStamina(potentialStamina);

        if(OnStaminaRestore != null)
            OnStaminaRestore(stamina, potentialStamina);
    }

    #endregion

    public bool GetHit(StealthAgent agent, int softDamage, int hardDamage, Vector3 hitUp, Vector3 force, DamageType damageType, ChakraType element, StatusChance[] statuses)
    {
        // Send hit events - Can be listened to by AI, AnimationManager, etc.
        if(OnHitBy != null)
            OnHitBy(agent);
        
        if(OnHit != null)
            OnHit(softDamage, hardDamage);
        
        if(OnForce != null)
            OnForce(force);
        
        // Calculate damage with resistances
        float damageMult = damageResistanceMatrix.GetResistanceMult(damageType);
        float elementMult = elementalResistanceMatrix.GetResistanceMult(element);

        softDamage = Mathf.FloorToInt(softDamage * damageMult * elementMult);
        hardDamage = Mathf.FloorToInt(hardDamage * damageMult * elementMult);

        Debug.Log(gameObject.name + " was hit for " + softDamage + " / " + hardDamage + " " + damageType + " damage");

        if(aliveVisual)
            DamagePopupManager.instance.Damage(softDamage, hardDamage, aliveVisual.transform.position + Vector3.up);
        
        SubHealth(softDamage);
        SubPotentialHealth(hardDamage);

        // Apply statuses
        if(statuses != null && statuses.Length > 0)
        {
            foreach(StatusChance s in statuses)
            {
                if(s.Success())
                    statusManager.AddStatus(s.status);
            }
        }

        return true;
    }

    public bool GetHeatDamage(float temperature)
    {
        if( temperature < minHeatDamageTemperature || 
            Time.time - lastHeatDamage < HeatDamageCooldown) 
            return false;

        Debug.Log(String.Format("HeatDamage from temp: {0}", temperature));

        SubHealth(heatSoftDamage);
        SubPotentialHealth(heatHardDamage);
        
        if(heatStatus != null)
            statusManager.AddStatus(heatStatus);

        lastHeatDamage = Time.time;

        return true;
    }

    public void StatusUpdate(int softHp, int hardHp, int softSt, int hardSt)
    {
        // HP
        if(softHp < 0)
            DepleteHealth(softHp * -1, 0);
        else if (softHp > 0)
            RestoreHealth(softHp, 0);

        if(hardHp < 0)
            DepleteHealth(0, hardHp * -1);
        else if (hardHp > 0)
            RestoreHealth(0, hardHp);
        
        // ST
        if(softSt < 0)
            DepleteStamina(softSt * -1);
        else if (softSt > 0)
            RestoreStamina(softSt, 0);

        if(hardSt < 0)
            DepletePotentailStamina(hardSt * -1);
        else if (hardSt > 0)
            RestoreStamina(0, hardHp);
    }

    public void StatusStart(Status status)
    {
        AddModifiers(status.AttributeModifiers.ToList());
        AddModifiers(status.ElementResistanceModifiers.ToList());
        //AddModifiers(status.DamageTypeResistanceModifiers.ToList());
    }

    public void StatusEnd(Status status)
    {
        RemoveModifiers(status.AllModifiers);
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
        if(chakraManager == null) 
            return 0;
        
        return chakraManager.GetChakraAmount(type);
    }

    public float GetRelationship(string factionName)
    {
        if(faction == null)
            return 0;
        return faction.GetRelationship(factionName);
    }
}
