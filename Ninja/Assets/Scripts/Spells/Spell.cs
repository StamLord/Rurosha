using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Spells/Spell", order = 0)]
public class Spell : ScriptableObject 
{
    public string spellName;

    public float spellCost;
    public ChakraType chakraType;
    
    public enum CastType {UNSTOPPABLE, STOPPABLE, CONTINUOUS}
    public CastType castType;

    public GameObject spellObject;
    public bool multiCast;

    public enum ParentType {NONE, TRANSFORM, CAMERA}
    public ParentType parentType;

    public bool inheritRotation;

    public int healAmount;

    public bool Cast(CharacterStats characterStats, out GameObject spellObject, out ParentType parentType, out bool inheritRotation)
    {
        spellObject = this.spellObject;
        parentType = this.parentType;
        inheritRotation = this.inheritRotation;

        if(characterStats.DepleteChakra(chakraType, spellCost) == false)
            return false;

        // Apply immediate effects
        characterStats.RestoreHealth(healAmount, 0);

        return true;
    }
}
