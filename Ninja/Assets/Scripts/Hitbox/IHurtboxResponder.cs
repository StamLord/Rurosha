using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurtboxResponder
{
    void GetHit(int softDamage, int hardDamage, DamageType damageType);
}
