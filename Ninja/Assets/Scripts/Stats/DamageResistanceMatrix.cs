using UnityEngine;

[System.Serializable]
public class DamageResistanceMatrix : ResistanceMatrix
{
    [SerializeField] private ElementalResistance blunt = ElementalResistance.C;
    [SerializeField] private ElementalResistance slash = ElementalResistance.C;
    [SerializeField] private ElementalResistance pierce = ElementalResistance.C;

    public ElementalResistance GetResistance(DamageType damageType)
    {
        switch(damageType)
        {
            case DamageType.Blunt:
                return blunt;
            case DamageType.Slash:
                return slash;
            case DamageType.Pierce:
                return pierce;
        }

        return blunt;
    }

    public float GetResistanceMult(DamageType damageType)
    {
        return GetResistanceMult(GetResistance(damageType));
    }
}
