using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "Trait", order = 0)]
public class Trait : ScriptableObject
{
    [SerializeField] private string traitName;
    [SerializeField] private string description;
    public List<Modifier> modifiers = new List<Modifier>();

    public string TraitName { get { return traitName; } }
    public string Description { get { return description; } }
}
