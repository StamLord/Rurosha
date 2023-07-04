using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxDelegate : MonoBehaviour
{
    [SerializeField] private Hurtbox hurtbox;

    public bool Hit(StealthAgent agent, int softDamage, int hardDamage, Vector3 hitUp, Vector3 force, DamageType damageType = DamageType.Blunt)
    {
        return hurtbox.Hit(agent, softDamage, hardDamage, hitUp, force, damageType);
    }
}
