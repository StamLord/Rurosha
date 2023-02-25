using UnityEngine;

public interface IHurtboxResponder
{
    bool GetHit(StealthAgent agent, int softDamage, int hardDamage, Vector3 hitUp, DamageType damageType, Status[] statuses);
}
