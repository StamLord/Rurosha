using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Spells/Spell", order = 0)]
public class Spell : ScriptableObject 
{
    public string spellName;
    public int spellCost;
    public GameObject spellObject;
    public bool multiCast;
    public bool casterParent;
    public bool inheritRotation;

    public int healAmount;

    public bool Cast(CharacterStats characterStats, out GameObject spellObject, out bool casterParent, out bool inheritRotation)
    {
        spellObject = this.spellObject;
        casterParent = this.casterParent;
        inheritRotation = this.inheritRotation;

        if(characterStats.DepleteStamina(spellCost) == false)
            return false;

        // Apply immediate effects
        characterStats.AddHealth(healAmount);

        return true;
    }
}
