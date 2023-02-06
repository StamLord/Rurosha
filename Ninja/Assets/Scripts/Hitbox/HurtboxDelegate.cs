using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxDelegate : MonoBehaviour
{
    [SerializeField] private Hurtbox hurtbox;

    public bool Hit(StealthAgent agent, int softDamage, int hardDamage, Vector3 hitUp, DamageType damageType = DamageType.Blunt)
    {
        return hurtbox.Hit(agent, softDamage, hardDamage, hitUp, damageType);
    }
}
