using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxDelegate : MonoBehaviour
{
    [SerializeField] private Hurtbox hurtbox;

    public bool Hit(StealthAgent agent, int softDamage, int hardDamage, DamageType damageType = DamageType.Blunt, Direction9 direction = Direction9.CENTER)
    {
        return hurtbox.Hit(agent, softDamage, hardDamage, damageType, direction);
    }
}
