using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Chakra[] baseChakras;
    [SerializeField] private Chakra[] advancedChakras;
    [SerializeField] private ChakraType focused;
    [SerializeField] Dictionary<ChakraType, Chakra> dict = new Dictionary<ChakraType, Chakra>();
    [SerializeField] private DayNightManager timeManager;

    [Header("Charging Rate")]
    [SerializeField] private float baseChargeRate = 1f; 
    [SerializeField] private float zodiacChargeMult = 2f; // When zodiac hour matches element
    [SerializeField] private float focusChargeMult = 1.5f; // When focused

    [Header("Charging Rate")]
    Dictionary<Zodiac, ChakraType> zodiacElements = new Dictionary<Zodiac, ChakraType> 
    {
        {Zodiac.RAT, ChakraType.WATER},
        {Zodiac.OX, ChakraType.VOID},
        {Zodiac.TIGER, ChakraType.WIND},
        {Zodiac.RABBIT, ChakraType.WIND},
        {Zodiac.DRAGON, ChakraType.VOID},
        {Zodiac.SNAKE, ChakraType.FIRE},
        {Zodiac.HORSE, ChakraType.FIRE},
        {Zodiac.GOAT, ChakraType.VOID},
        {Zodiac.MONKEY, ChakraType.EARTH},
        {Zodiac.ROOSTER, ChakraType.EARTH},
        {Zodiac.DOG, ChakraType.VOID},
        {Zodiac.BOAR, ChakraType.WATER},
    };

    private float lastTimeUpdate;

    private void Start() 
    {
        lastTimeUpdate = timeManager.GetTime();

        foreach(Chakra c in baseChakras)    
            dict[c.Type] = c;

        foreach(Chakra c in advancedChakras)    
            dict[c.Type] = c;
    }

    private Chakra GetChakra(ChakraType type)
    {
        if(dict.ContainsKey(type))
            return dict[type];
        
        return null;
    }

    public float GetChakraAmount(ChakraType type)
    {
        if(dict.ContainsKey(type))
            return dict[type].TotalAmount;
        
        return -1;
    }

    private void Update() 
    {
        // Get amount of time passed since last update
        float time = timeManager.GetTime();
        float delta = time - lastTimeUpdate;

        // Convert to hours
        delta /= 3600;

        // Add charge amount for time passed
        foreach(Chakra c in baseChakras)
            Charge(c, delta * baseChargeRate);

        lastTimeUpdate = time;
    }

    private void Charge(Chakra chakra, float amount)
    {
        float mult = 1;

        Zodiac z = timeManager.GetZodiacHour();
        if(zodiacElements.ContainsKey(z))
        {
            if(chakra.Type == zodiacElements[z])
                mult *= zodiacChargeMult;
        }

        if(chakra.Type == focused)
            mult *= focusChargeMult;

        chakra.Add(amount * mult);
    }

    public bool DepleteChakra(ChakraType type, float amount)
    {
        if(dict.ContainsKey(type))
        {
            bool enough = amount <= dict[type].TotalAmount;
            if(enough)
            {
                dict[type].Remove(amount);
                return true;
            }
        }

        return false;
    }

    public void Focus(ChakraType type)
    {
        focused = type;
    }

    public void Convert(ChakraType from, ChakraType to, float amount, float rate)
    {
        dict[from].Remove(amount);
        dict[to].Add(amount * rate);
    }
}
