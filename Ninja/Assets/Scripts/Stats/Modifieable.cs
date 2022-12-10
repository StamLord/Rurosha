using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifieable
{
    [SerializeField] protected List<Modifier> modifiers = new List<Modifier>();

    public enum RoundType {DOWN, UP}
    [SerializeField] protected RoundType roundType;

    public abstract int CalculateModified();

    public abstract bool AddModifier(Modifier modifier);

    public abstract bool RemoveModifier(Modifier modifier);
}
