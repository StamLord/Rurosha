using System.Collections;
using UnityEngine;

public class WeaponsUI : UISubWindow
{
    [SerializeField] private WeaponSlotUI[] _slots;
    [SerializeField] private WeaponManager _wManager;

    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = .5f;

    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float dissapearTime = 1f;

    [SerializeField] private int lastIndex = 0;
    
    private Coroutine autohide;
    private bool autohideRunning;

    private void Start()
    {
        _wManager.ChangeSelectionEvent += UpdateSelection;
        _wManager.ChangeItemEvent += UpdateSlotItem;
    }

    public override void Open()
    {
        base.Open();

        // If we open this window using WindowUI framework we stop countdown to auto hide
        StopAutoHide();
        
        // Set all slots to visible
        for (int i=0; i < _slots.Length; i++)
            ChangeSlotOpacity(i, 1);

        for (int i=0; i < _slots.Length; i++)
            SetSlot(i, i == lastIndex);
    }

    public override void Close()
    {
        base.Close();

        // If we close this window using WindowUI framework we stop countdown to auto hide
        StopAutoHide();
    }

    private void StartAutoHide()
    {
        StopAutoHide();
        autohide = StartCoroutine(AutoHideUI());
    }

    private void StopAutoHide()
    {
         if(autohideRunning)
            StopCoroutine(autohide);
    }

    private IEnumerator AutoHideUI()
    {
        autohideRunning = true;

        yield return new WaitForSecondsRealtime(displayTime);

        float startTime = Time.time;

        while(Time.time - startTime <= dissapearTime)
        {
            float t = (Time.time - startTime) / dissapearTime;
            for (int i=0; i < _slots.Length; i++)
                ChangeSlotOpacity(i, 1 - t);

            yield return null;
        }
        
        // Close UIWindow
        Close();

        autohideRunning = false;
    }

    private void UpdateSelection(int index)
    {
        lastIndex = index;

        Open();
        StartAutoHide();
    }

    private void SetSlot(int index, bool active)
    {
        _slots[index].SetActive(active);
        ChangeSlotOpacity(index, 1);
    }

    private void ChangeSlotOpacity(int index, float precentage)
    {
        _slots[index].ChangeSlotOpacity(precentage);
    }

    private void UpdateSlotItem(int index, Item item, int stack)
    {
        Open();
        StartAutoHide();

        _slots[index].UpdateItem(item, stack);
    }
}
