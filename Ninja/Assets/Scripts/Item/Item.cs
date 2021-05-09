using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
public class Item : ScriptableObject
{
    public Mesh model;
    public string itemName;

    public bool stackable;

    [SerializeField] private int _ammo;
    public int ammo { get { return _ammo; } set { _ammo = Mathf.Max(0, value); } }
}
