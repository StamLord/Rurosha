using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    [SerializeField] private List<Trait> boons = new List<Trait>();
    [SerializeField] private List<Trait> flaws = new List<Trait>();

    private Dictionary<string, Trait> boonDict = new Dictionary<string, Trait>();
    private Dictionary<string, Trait> flawDict = new Dictionary<string, Trait>();
    private bool initialized;
    
    #region Singleton

    public static TraitManager instance;

    private void Awake() 
    {
        if(instance != null)
        {
            Debug.LogWarning("More than 1 instance of TraitManager exists. Destroying: " + gameObject.name);
            Destroy(this);
        }
        else
            instance = this;
    }
    
    #endregion

    private void InitializeDictionaries()
    {
        for (int i = 0; i < boons.Count; i++)
            boonDict.Add(boons[i].TraitName, boons[i]);

        for (int i = 0; i < flaws.Count; i++)
            flawDict.Add(flaws[i].TraitName, flaws[i]);
        
        initialized = true;
    }

    public Trait GetBoon(string name)
    {
        if(boonDict.ContainsKey(name))
            return boonDict[name];
        else
            return null;
    }

    public Trait GetFlaw(string name)
    {
        if(flawDict.ContainsKey(name))
            return flawDict[name];
        else
            return null;
    }

    public List<string> GetBoonList()
    {
        if(initialized == false)
            InitializeDictionaries();
        
        return new List<string>(boonDict.Keys);
    }

    public List<string> GetFlawList()
    {
        if(initialized == false)
            InitializeDictionaries();
        
        return new List<string>(flawDict.Keys);
    }
}
