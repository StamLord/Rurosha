using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private List<Spell> learned = new List<Spell>();
    [SerializeField] private Spell[] prepared;
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private new Camera camera;

    Dictionary<string, SpellObject> spellObjects = new Dictionary<string, SpellObject>();

    public bool Learn(Spell spell)
    {
        if(learned.Contains(spell))
            return false;

        learned.Add(spell);
        return true;
    }

    public bool PrepareSpell(string spellName, int index)
    {
        if(index >= prepared.Length)
            return false;
        
        Spell s = GetSpell(spellName);
        if(s == null)
            return false;

        prepared[index] = s;
        return true;
    }

    private Spell GetSpell(string spellName)
    {
        foreach(Spell s in learned)
            if(s.spellName == spellName)
                return s;
        
        return null;
    }

    public string GetPreparedSpellName(int index)
    {
        if(index >= prepared.Length || prepared[index] == null)
            return null;

        return prepared[index].spellName;
    }

    public bool Cast(int preparedIndex)
    {
        Spell spell = prepared[preparedIndex];

        GameObject spellObject;
        bool casterParent;
        bool inheritRotation;
        bool success = spell.Cast(characterStats, out spellObject, out casterParent, out inheritRotation);
        
        if(success == false)
            return false;

        // If already instantiated just activate
        if(spellObjects.ContainsKey(spell.spellName))
        {
            SpellObject spellObj = spellObjects[spell.spellName];

            // Position
            spellObj.transform.position = characterStats.transform.position + spellObj.offset;
            
            // Rotation
            if(inheritRotation)
                if(camera)
                    spellObj.transform.rotation = camera.transform.rotation;
                else
                    spellObj.transform.rotation = characterStats.transform.rotation;
            
            spellObj.Activate(this);
        }
        // Instantiate and keep reference for future activation
        else
        {
            GameObject obj = Instantiate(spellObject, characterStats.transform.position, Quaternion.identity);

            if(casterParent)
                obj.transform.SetParent(characterStats.transform);

            // Rotation
            if(inheritRotation)
                if(camera)
                    obj.transform.rotation = camera.transform.rotation;
                else
                    obj.transform.rotation = characterStats.transform.rotation;

            SpellObject spellObj = obj.GetComponent<SpellObject>();
            
            // Position
            obj.transform.position += spellObj.offset;

            if(spellObj)
            {
                spellObjects[spell.spellName] = spellObj;
                spellObj.Activate(this);
            }
        }

        return true;
    }
}
