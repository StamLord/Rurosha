using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Status", order = 6)]
public class Status : ScriptableObject
{
    [SerializeField] private string statusName;
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;
    [SerializeField] private int cycles;
    [SerializeField] private float updateRate;
    [SerializeField] private int hpChange;
    [SerializeField] private int stChange;
    [SerializeField] private Status[] cures;
    [SerializeField] private Status[] prevents;
    [SerializeField] private AttributeModifier[] attributeModifiers;
    [SerializeField] private ElementResistanceModifier[] elementResistanceModifiers;
    [SerializeField] private DamageTypeResistanceModifier[] damageTypeResistanceModifiers;

    public string Name {get{return statusName;}}
    public Sprite Icon {get{return icon;}}
    public string Description {get{return description;}}
    public int Cycles {get{return cycles;}}
    public float UpdateRate {get{return updateRate;}}
    public int HpChange {get{return hpChange;}}
    public int StChange {get{return stChange;}}
    public Status[] Cures {get{return cures;}}
    public Status[] Prevents {get{return prevents;}}

    public AttributeModifier[] AttributeModifiers {get{return attributeModifiers;}}
    public ElementResistanceModifier[] ElementResistanceModifiers {get{return elementResistanceModifiers;}}
    public DamageTypeResistanceModifier[] DamageTypeResistanceModifiers {get{return damageTypeResistanceModifiers;}}

    private List<Modifier> allModifiers = null;

    public List<Modifier> AllModifiers { get 
    {
        // Calculate only once when allModifiers is null
        if(allModifiers != null) return allModifiers;

        List<Modifier> modifiers = new List<Modifier>();

        foreach (var mod in attributeModifiers)
            modifiers.Add(mod.modifier);
        foreach (var mod in elementResistanceModifiers)
            modifiers.Add(mod.modifier);
        foreach (var mod in damageTypeResistanceModifiers)
            modifiers.Add(mod.modifier);

        allModifiers = modifiers;
        
        return allModifiers;
    }}
}
