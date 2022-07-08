using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurtboxResponder
{
    bool GetHit(StealthAgent agent, int softDamage, int hardDamage, DamageType damageType, Direction9 direction);
}
