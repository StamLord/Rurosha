using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakraManager : MonoBehaviour
{
    [SerializeField] private Chakra[] baseChakras;
    [SerializeField] private Chakra[] advancedChakras;
    [SerializeField] private ChakraType focused;
    [SerializeField] Dictionary<ChakraType, Chakra> dict = new Dictionary<ChakraType, Chakra>();
    [SerializeField] private DayNightManager timeManager;
    [SerializeField] private float chargeRate = 1f; // Per DayNightManager hour

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
            c.Add(delta * chargeRate);

        lastTimeUpdate = time;
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
