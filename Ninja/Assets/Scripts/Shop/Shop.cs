using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Shop : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private int money;
    [SerializeField] private Item[] inventory;
    [SerializeField] private Queue<Item> sold = new Queue<Item>();

    [Header("Visual")]
    [SerializeField] private Transform worldParent;
    [SerializeField] private Vector3 rowOffset;
    [SerializeField] private Vector3 colOffset;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private int maxRows;
    [SerializeField] private int maxCols;
    [SerializeField] private GameObject[] worldItems;
    [SerializeField] private bool drawGizmos;

    [Header("VFX")]
    [SerializeField] private Transform coinOrigin;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float coinInterval = .2f;
    [SerializeField] private float coinLifetime= 3f;
    [SerializeField] private Vector3 minRotation;
    [SerializeField] private Vector3 maxRotation;
    [SerializeField] private int coinStartAmount;
    private Pool pool;
    

    private void Start() 
    {
        UpdateAllVisual();

        pool = new Pool(coinPrefab, coinStartAmount);
    }

    private void UpdateAllVisual() 
    {        
        int n = maxRows * maxCols;
        n = Mathf.Min(n, inventory.Length);

        for(int i = 0; i < n; i++)
            UpdateVisual(i);
    }

    private void ClearAllVisual()
    {
        for (var i = 0; i < worldItems.Length; i++)
        {
            Destroy(worldItems[i]);
            worldItems[i] = null;
        }
    }

    private void UpdateVisual(int index)
    {
        // Remove
        if(inventory[index] == null && worldItems[index] != null)
        {
            Destroy(worldItems[index]);
            worldItems[index] = null;
            return;
        }

        // Calculate row and col
        int row = Mathf.FloorToInt(index / maxCols);
        int col = index % maxCols;
        Vector3 pos = worldParent.position + rowOffset * row + colOffset * col;

        // Add
        if(inventory[index] != null && worldItems[index] == null)
        {
            // Instantiate
            worldItems[index] = Instantiate(inventory[index].pickup, pos, Quaternion.Euler(rotation), worldParent);
            
            // Set references
            Pickup pickup = worldItems[index].GetComponent<Pickup>();
            if(pickup)
            { 
                pickup.SetItem(inventory[index]);
                pickup.OnAttemptPickup += CanBuy;
                pickup.OnPickup += Buy;
            }
            return;
        }

        // Update
        if(inventory[index] != null && worldItems[index] != null)
        {
            worldItems[index].transform.position = pos;
            worldItems[index].transform.rotation = Quaternion.Euler(rotation);
        }
    }

    public bool CanBuy(Item item, Interactor interactor)
    {
        // Check if enough money
        return true;
    }

    public void Buy(Item item)
    {
        int index = -1;

        // Find item index
        for (var i = 0; i < inventory.Length; i++)
        {
            if(inventory[i] == item)
            {
                index = i;
                break;
            }
        }

        if(index < 0)
            return;

        money += item.cost;
        inventory[index] = null;
        sold.Enqueue(item);

        StartCoroutine("CoinVFX", item.cost);
    }

    private IEnumerator CoinVFX(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            StartCoroutine("SingleCoin", coinLifetime);
            yield return new WaitForSeconds(coinInterval);
        }
    }

    private IEnumerator SingleCoin(float lifetime)
    {
        GameObject o = pool.Get();
        o.transform.position = coinOrigin.position;

        // Randomize rotaiton
        Vector3 rotation = new Vector3(
            Mathf.Lerp(minRotation.x, maxRotation.x, Random.value),
            Mathf.Lerp(minRotation.y, maxRotation.y, Random.value),
            Mathf.Lerp(minRotation.z, maxRotation.z, Random.value));
        
        o.transform.rotation = Quaternion.Euler(rotation);

        yield return new WaitForSeconds(lifetime);
        pool.Return(o);
    }

    private void OnDrawGizmos() 
    {
        if(drawGizmos == false) return;

        Gizmos.color = Color.green;

        for (var i = 0; i < maxRows; i++)
            for (var j = 0; j < maxCols; j++)
                Gizmos.DrawCube(worldParent.position + rowOffset * i + colOffset* j, Vector3.one * .2f);
    }
}
