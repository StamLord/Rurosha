using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
public class Item : ItemDefinition
{
    [Header("Visual")]
    public Mesh model;
    public Material material;

    [Header("Data")]
    public string itemName;
    public bool stackable;
    [SerializeField] private int _amount = 1;
    public int amount { get { return _amount; } set { _amount = Mathf.Max(0, value); } }
    public float durability = 1;
    public int cost = 1;

    [Header("Pickup")]
    public GameObject pickup;

    [Header("Projectile")]
    public GameObject projectile;

    public virtual void Randomize()
    {

    }
}
