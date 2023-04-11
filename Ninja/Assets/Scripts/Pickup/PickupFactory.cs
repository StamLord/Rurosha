using System.Collections.Generic;
using UnityEngine;
using System;

public class PickupFactory : MonoBehaviour
{
    #region Singleton
    
    public static PickupFactory instance;
    
    private void Awake() 
    {
        if(instance != null)
        {
            Debug.LogWarning("More than 1 instance of PickupFactory exists. Destroying object" + gameObject.name);
            Destroy(gameObject);
            return;
        }
    
        instance = this;
    }
    
    #endregion

    private Dictionary<string, Pool> pickupPools = new Dictionary<string, Pool>();
    private Dictionary<string, Pool> coinPools = new Dictionary<string, Pool>();

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject coinPursePrefab;

    public GameObject GetPickup(Item item)
    {
        if(item == null) return null;

        if(pickupPools.ContainsKey(item.itemName) == false)
            pickupPools[item.itemName] = new Pool(item.pickup);

        GameObject go = pickupPools[item.itemName].Get();

        // Set the unique values like random colors
        Pickup p = go.GetComponent<Pickup>();
        (p)?.SetItem(item);

        return go;
    }

    public GameObject GetCoinPickup(int amount)
    {
        if(amount < 1)
            return null;
        
        GameObject prefab = (amount > 1)? coinPursePrefab : coinPrefab;

        if(coinPools.ContainsKey(prefab.name) == false)
            coinPools[name] = new Pool(prefab);

        GameObject go = coinPools[prefab.name].Get();

        // Set the amount of money
        Pickup p = go.GetComponent<Pickup>();
        p?.SetMoney(amount);

        return go;
    }

    public void DestroyPickup(Pickup pickup)
    {
        Item item = pickup.Item;
        bool isMoney = pickup.IsMoney;

        if(item != null)
        {
            if(pickupPools.ContainsKey(item.itemName))
                pickupPools[item.itemName].Return(pickup.gameObject);
            else
            {
                pickupPools[item.itemName] = new Pool(item.pickup, 9);
                pickupPools[item.itemName].Return(pickup.gameObject);
            }
        }
        else if(isMoney)
        {
            if(coinPursePrefab == null || coinPrefab == null)
                throw new ArgumentException("coinPursePrefab and coinPrefab cannot be null");

            GameObject prefab = (pickup.Money > 1)? coinPursePrefab : coinPrefab;

            if(coinPools.ContainsKey(prefab.name))
                coinPools[prefab.name].Return(pickup.gameObject);
            else
            {
                coinPools[prefab.name] = new Pool(prefab, 9);
                coinPools[prefab.name].Return(pickup.gameObject);
            }
        }
    }
}
