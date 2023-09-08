public class ResistanceMatrix
{
    public static float GetResistanceMult(Resistance resistance)
    {
        switch(resistance)
        {
            case Resistance.A:
                return 0;
            case Resistance.B:
                return .5f;
            case Resistance.C:
                return 1f;
            case Resistance.D:
                return 1.5f;
            case Resistance.E:
                return 2f;
        }

        return 1f;
    }
}
