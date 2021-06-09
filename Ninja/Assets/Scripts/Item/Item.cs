using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
public class Item : ScriptableObject
{
    public Mesh model;
    public Material material;

    public string itemName;

    public bool stackable;

    [SerializeField] private int _ammo = 1;
    public int ammo { get { return _ammo; } set { _ammo = Mathf.Max(0, value); } }

    public float durability = 100f;
}
