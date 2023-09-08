using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "Trait", order = 0)]
public class Trait : ScriptableObject
{
    [SerializeField] private string traitName;
    [SerializeField] private string description;
    public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
    public List<ElementResistanceModifier> elementalResistanceModifiers = new List<ElementResistanceModifier>();
    public List<DamageTypeResistanceModifier> damageTypeResistanceModifiers = new List<DamageTypeResistanceModifier>();

    private List<Modifier> allModifiers = null;

    public List<Modifier> AllModifiers { get 
    {
        // Calculate only once when allModifiers is null
        if(allModifiers != null) return allModifiers;

        List<Modifier> modifiers = new List<Modifier>();

        foreach (var mod in attributeModifiers)
            modifiers.Add(mod.modifier);
        foreach (var mod in elementalResistanceModifiers)
            modifiers.Add(mod.modifier);
        foreach (var mod in damageTypeResistanceModifiers)
            modifiers.Add(mod.modifier);

        allModifiers = modifiers;
        
        return allModifiers;
    }}

    public string TraitName { get { return traitName; } }
    public string Description { get { return description; } }
}
