using UnityEngine;
using FarrokhGames.Inventory;

public abstract class QuickSlotsBase : MonoBehaviour
{
    public abstract IInventoryItem GetItem(int index);

    public abstract bool RemoveItem(int index);

    public abstract bool AddItem(int index, IInventoryItem item);

    public abstract bool AddItemAtFirstEmpty(IInventoryItem item);

    public abstract int Length();

    #region Events

    public delegate void itemsUpdated();
    public abstract event itemsUpdated OnItemsUpdated;

    public delegate void itemAddDelegate(int index);
    public abstract event itemAddDelegate OnItemAdd;

    public delegate void itemRemoveDelegate(int index);
    public abstract event itemRemoveDelegate OnItemRemove;

    public delegate void itemDropdelegate(IInventoryItem item);
    public abstract event itemDropdelegate OnItemDropped;

    #endregion

    #region Operators

    public abstract IInventoryItem this[int index] { get; }

    #endregion
}
