using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trait
{
    [SerializeField] private string traitName;
    [SerializeField] private string description;

    public string TraitName { get { return traitName;} }
}
