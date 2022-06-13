using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "Faction", order = 0)]
public class Faction : ScriptableObject 
{
    [SerializeField] private string factionName;

    // Pairs faction names with relationship values (Floats between -1f and 1f)
    [Header ("Relationships")]
    [SerializeField] private List<string> factionNames = new List<string>();
    [SerializeField] private List<float> relations = new List<float>();

    private Dictionary<string, float> relationship = new Dictionary<string, float>();
    private bool initialized;

    public string FactionName { get {return factionName.ToLower();}}
    
    public void Initialize()
    {
        // Convert 2 lists to dictionary<string,float>
        for(int i = 0; i < Mathf.Min(factionNames.Count, relations.Count); i++)
            relationship[factionNames[i]] = relations[i];
        initialized = true;
    }

    public float GetRelationship(string factionName)
    {
        if(initialized == false) Initialize();
        if(relationship.ContainsKey(factionName))
            return relationship[factionName];
        return 0f;
    }

    public void ChangeRelationship(string factionName, float amount)
    {
        if(initialized == false) Initialize();
        if(relationship.ContainsKey(factionName))
            relationship[factionName] += amount;
        else
            relationship[factionName] = amount;

        relationship[factionName] = Mathf.Clamp(relationship[factionName], -1f, 1f);
    }
}
