using UnityEngine;

public interface IHurtboxResponder
{
    bool GetHit(StealthAgent agent, int softDamage, int hardDamage, Vector3 hitUp, Vector3 force, DamageType damageType, ChakraType element, StatusChance[] statuses);

    bool GetHeatDamage(float temperature);
}
