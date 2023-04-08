using System.Collections.Generic;
using UnityEngine;

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

    public void DestroyPickup(Pickup pickup)
    {
        Item item = pickup.Item;
        if(pickupPools.ContainsKey(item.itemName))
            pickupPools[item.itemName].Return(pickup.gameObject);
        else
        {
            pickupPools[item.itemName] = new Pool(item.pickup, 9);
            pickupPools[item.itemName].Return(pickup.gameObject);
        }   
    }
}
