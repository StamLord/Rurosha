using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    [SerializeField] private List<Faction> factions = new List<Faction>();

    #region Singleton
    public static FactionManager instance;
    
    private void Awake() 
    {
        if(instance != null)    
        {
            Debug.LogWarning("More than 1 instace of FactionManager exists!");
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        foreach(Faction f in factions)
            f.Initialize();
    }

    #endregion

    public float GetRelationship(string ofFaction, string withFaction)
    {
        Faction f = GetFaction(ofFaction);
        if(f == null)
            return -1f;
        
        return f.GetRelationship(withFaction);
    }

    public bool ChangeRelationship(string ofFaction, string withFaction, float amount)
    {
        Faction f = GetFaction(ofFaction);
        if(f == null)
            return false;
        
        f.ChangeRelationship(withFaction, amount);
        return true;
    }

    private Faction GetFaction(string factionName)
    {
        factionName = factionName.ToLower();
        foreach(Faction f in factions)
            if(f.FactionName == factionName)
                return f;
        return null;
    }
}