using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {MELEE, KNIFE, SWORD, STAFF, KANABO, SHURIKEN, BIG_SHURIKEN, BOMB, BOW, ITEM}

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon", order = 2)]
public class Weapon : Item
{
    [SerializeField] public WeaponType WeaponType;

}
