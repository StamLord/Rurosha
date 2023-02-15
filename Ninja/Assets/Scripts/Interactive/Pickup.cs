using UnityEngine;

public class Pickup : PhysicalObject
{
    [Header ("Item")]
    [SerializeField] private Item item;
    [SerializeField] private bool randomize;

    [Header ("Money")]
    [SerializeField] private bool money;
    [SerializeField] private int moneyAmount;

    [Header ("Visual")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    public delegate bool attemptPickupDelegate(Item item, Interactor interactor);
    public event attemptPickupDelegate OnAttemptPickup;

    public delegate void pickupDelegate(Item item);
    public event pickupDelegate OnPickup;

    private void Awake() 
    {
        InitializeOutline();
        
        if(randomize)
        {
            item = Instantiate(item); // Create instance so we change the copy and not the default item
            item.Randomize();
            UpdateVisual();
        }
    }

    public void SetItem(Item item)
    {
        this.item = item;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if(item is Equipment)
        {
            Equipment e = (Equipment)item;
            if(meshRenderer)
            {
                Equipment.Palette p = e.GetPalette();
                meshRenderer.material.SetColor("_Main_Color", p.primary);
                meshRenderer.material.SetColor("_Secondary_Color", p.secondary);
                meshRenderer.material.SetTexture("_Secondary_Pattern", e.GetPattern());
            }
        }
    }

    public override void Use(Interactor interactor)
    {
        base.Use(interactor);
        
        if(money)
        {
            interactor.AddMoney(moneyAmount);
            if(OnPickup != null)
                    OnPickup(item);
            
            DestroyPickup();
        }

        if(item)
        {
            if(OnAttemptPickup != null)
            {
                bool success = OnAttemptPickup(item, interactor);
                if(success == false) return;
            }

            if(interactor.AddItem(item, this))
            {
                if(OnPickup != null)
                    OnPickup(item);
                
                DestroyPickup();
            }
        }
    }

    private void DestroyPickup()
    {
        // If we have child pickups,
        // we unparent them so they are not destroyed
        Projectile[] childPickups = GetComponentsInChildren<Projectile>();
        foreach(Projectile proj in childPickups)
        {
            if(proj.transform == transform) continue; // Ignore ourselves
            GameObject go = proj.ReplaceWithPickup();
            Pickup p = go.GetComponent<Pickup>();
            if(p == null) continue;
            p.transform.SetParent(transform.parent);
            p.SetRigidActive(true);
        }

        Destroy(transform.gameObject);
    }

    // Used by Grappling Hook to pick up items
    public void Use(Inventory manager)
    {
        if(item)
            if(manager.AddItem(item))
                Destroy(transform.gameObject);
    }
}
