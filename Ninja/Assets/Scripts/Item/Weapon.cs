using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon", order = 2)]
public class Weapon : Item
{
    [SerializeField] public WeaponType WeaponType;
    [SerializeField] public int damage;

}
