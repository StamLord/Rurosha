public class ResistanceMatrix
{
    public static float GetResistanceMult(ElementalResistance resistance)
    {
        switch(resistance)
        {
            case ElementalResistance.A:
                return 0;
            case ElementalResistance.B:
                return .5f;
            case ElementalResistance.C:
                return 1f;
            case ElementalResistance.D:
                return 1.5f;
            case ElementalResistance.E:
                return 2f;
        }

        return 1f;
    }
}
