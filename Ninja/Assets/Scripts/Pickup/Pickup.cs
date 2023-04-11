using UnityEngine;

public class Pickup : PhysicalObject
{
    [Header ("Owner")]
    [SerializeField] private CharacterStats owner;
    [SerializeField] public CharacterStats Owner { get {return owner;}}

    [Header ("Item")]
    [SerializeField] private Item item;
    [SerializeField] private bool randomize;
    [SerializeField] private UIItemPrice costObject;

    public Item Item => item;

    [Header ("Money")]
    [SerializeField] private bool isMoney;
    [SerializeField] private int moneyAmount;

    public bool IsMoney => isMoney;
    public int Money => moneyAmount;

    [Header ("Visual")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    public delegate bool attemptPickupDelegate(Item item, Interactor interactor);
    public event attemptPickupDelegate OnAttemptPickup;

    public delegate void pickupDelegate(Item item, Interactor interactor);
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

    public void SetMoney(int amount)
    {
        isMoney = true;
        moneyAmount = amount;
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
        
        bool isSteal = IsStealing(interactor);

        if(isMoney)
        {
            interactor.AddMoney(moneyAmount);
            if(OnPickup != null)
                OnPickup(item, interactor);
            
            if(isSteal) interactor.CommitSteal();

            DestroyPickup();
        }

        if(item)
        {   
            // Unless we steal, call OnAttemptPickup to check if possible (enough money etc.)
            if(isSteal == false && OnAttemptPickup != null)
            {
                bool success = OnAttemptPickup(item, interactor);
                if(success == false) return;
            }

            if(interactor.AddItem(item))
            {
                if(OnPickup != null)
                    OnPickup(item, interactor);
                
                if(isSteal) interactor.CommitSteal();
                
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

        if(PickupFactory.instance != null)
            PickupFactory.instance.DestroyPickup(this);
        else
            Destroy(transform.gameObject);
    }

    // Used by Grappling Hook to pick up items
    public void Use(QuickSlots quickSlots)
    {
        if(item)
            if(quickSlots.AddItemAtFirstEmpty(item))
                Destroy(transform.gameObject);
    }

    public override string GetText(Interactor interactor = null)
    {
        if (IsStealing(interactor))
            return "Steal";

        return interactionText;
    }

    private bool IsStealing(Interactor interactor)
    {
        if(interactor == null) 
            return false;

        // TODO: Implement check if interactor is ally of owner
        return owner != null;
    }

    public void SetVisibleCost(bool visible)
    {
        if(costObject) 
        {
            costObject.gameObject.SetActive(visible);
            costObject.UpdatePrice(item.cost);
        }
    }
}
