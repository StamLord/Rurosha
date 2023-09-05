[System.Serializable]
public struct AttackInfo
{
    public int softDamage;
    public int hardDamage;
    public StatusChance[] statuses;
    public DamageType damageType;
    public ChakraType element;

    public AttackInfo(int softDamage, int hardDamage, DamageType damageType = DamageType.Blunt, ChakraType element = ChakraType.VOID)
    {
        this.softDamage = softDamage;
        this.hardDamage = hardDamage;
        this.damageType = damageType;
        this.element = element;
        statuses = new StatusChance[0];
    }
}
