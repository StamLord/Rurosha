using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private List<Spell> learned = new List<Spell>();
    [SerializeField] private Spell[] prepared;
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private StealthAgent stealthAgent;
    public StealthAgent Agent { get { return stealthAgent;}}
    [SerializeField] private new Camera camera;
    
    [SerializeField] private Spell activeSpell;
    [SerializeField] private SpellObject activeObject;

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

    public Spell GetPreparedSpell(int index)
    {
        if(index >= prepared.Length || prepared[index] == null)
            return null;

        return prepared[index];
    }

    public bool Cast(int preparedIndex)
    {
        Spell spell = prepared[preparedIndex];

        GameObject spellObject;
        Spell.ParentType parentType;
        bool inheritRotation;
        bool success = spell.Cast(characterStats, out spellObject, out parentType, out inheritRotation);
        
        if(success == false)
            return false;

        // If already instantiated just activate
        if(spellObjects.ContainsKey(spell.spellName))
        {
            SpellObject spellObj = spellObjects[spell.spellName];

            // Position and rotate
            switch(parentType)
            {
                case Spell.ParentType.TRANSFORM:
                    spellObj.transform.SetParent(characterStats.transform);
                    spellObj.transform.localPosition = spellObj.offset;
                    if(inheritRotation)
                        spellObj.transform.rotation = characterStats.transform.rotation;
                    break;
                case Spell.ParentType.CAMERA:
                    spellObj.transform.SetParent(camera.transform);
                    spellObj.transform.localPosition = spellObj.offset;
                    if(inheritRotation)
                        spellObj.transform.rotation = camera.transform.rotation;
                    break;
            }
            
            spellObj.Activate(this);
            activeObject = spellObj;
        }
        // Instantiate and keep reference for future activation
        else
        {
            GameObject obj = Instantiate(spellObject, characterStats.transform.position, Quaternion.identity);
            SpellObject spellObj = obj.GetComponent<SpellObject>();

            // Parent, position and rotate
            switch(parentType)
            {
                case Spell.ParentType.TRANSFORM:
                    obj.transform.SetParent(characterStats.transform);
                    obj.transform.localPosition = spellObj.offset;
                    if(inheritRotation)
                        obj.transform.rotation = characterStats.transform.rotation;
                    break;
                case Spell.ParentType.CAMERA:
                    obj.transform.SetParent(camera.transform);
                    obj.transform.localPosition = spellObj.offset;
                    if(inheritRotation)
                        obj.transform.rotation = camera.transform.rotation;
                    break;
            }

            if(spellObj)
            {
                spellObjects[spell.spellName] = spellObj;
                spellObj.Activate(this);
                activeObject = spellObj;
            }
        }

        activeSpell = spell;
        return true;
    }

    public void Stop()
    {
        if(activeObject)
        {
            activeObject.Stop();
            activeObject = null;
        }

        if(activeSpell)
            activeSpell = null;
    }
}
