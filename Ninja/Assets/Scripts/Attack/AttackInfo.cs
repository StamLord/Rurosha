public struct AttackInfo
{
    public int softDamage;
    public int hardDamage;
    public Status[] statuses;
    public DamageType damageType;

    public AttackInfo(int softDamage, int hardDamage, DamageType damageType = DamageType.Blunt)
    {
        this.softDamage = softDamage;
        this.hardDamage = hardDamage;
        this.damageType = damageType;
        statuses = new Status[0];
    }
    
    public AttackInfo(int softDamage, int hardDamage, Status[] statuses, DamageType damageType = DamageType.Blunt)
    {
        this.softDamage = softDamage;
        this.hardDamage = hardDamage;
        this.statuses = statuses;
        this.damageType = damageType;
    }

    public AttackInfo(int softDamage, int hardDamage, DamageType damageType, Status[] statuses)
    {
        this.softDamage = softDamage;
        this.hardDamage = hardDamage;
        this.damageType = damageType;
        this.statuses = statuses;
    }
}
