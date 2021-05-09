using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsUI : MonoBehaviour
{
    [SerializeField] private WeaponSlotUI[] _slots;
    [SerializeField] private WeaponManager _wManager;

    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = .5f;

    [SerializeField] private float displayStartTime;
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float dissapearSpeed = 1f;

    [SerializeField] private float dissappearPercentage;

    [SerializeField] private int lastIndex = 0;
    void Start()
    {
        _wManager.ChangeSelectionEvent += UpdateSelection;
        _wManager.ChangeItemEvent += UpdateSlotItem;
    }

    void Update()
    {
        if(Time.time - displayStartTime > displayTime)
        {    
            dissappearPercentage = Mathf.Min(1, dissappearPercentage + Time.deltaTime * dissapearSpeed);
            for (int i=0; i < _slots.Length; i++)
                ChangeSlotOpacity(i, 1 - dissappearPercentage);
        }
    }

    void UpdateSelection(int index)
    {
        displayStartTime = Time.time;
        dissappearPercentage = 0;
        lastIndex = index;

        for (int i=0; i < _slots.Length; i++)
            SetSlot(i, i == index);
    }

    void SetSlot(int index, bool active)
    {
        _slots[index].SetActive(active);
        ChangeSlotOpacity(index, 1);
    }

    void ChangeSlotOpacity(int index, float precentage)
    {
        _slots[index].ChangeSlotOpacity(precentage);
    }

    void UpdateSlotItem(int index, Item item, int stack)
    {
        displayStartTime = Time.time;
        dissappearPercentage = 0;

        _slots[index].UpdateItem(item, stack);
    }
}
