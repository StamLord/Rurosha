using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _stackNumber;

    private Color _itemTextColor;
    private Color _stackTextColor;
    private Color _inactiveColor;
    [SerializeField] private Color _activeColor;

    [SerializeField] private bool _active;

    void Start()
    {
        _itemTextColor = _itemName.color;
        _stackTextColor = _stackNumber.color;
        _inactiveColor = _image.color;
    }

    public void SetActive(bool active)
    {
        _active = active;
    }

    public void UpdateItem(Item item, int stack)
    {
        _itemName.text = (item)? item.itemName : "";
        _stackNumber.text = (stack == 0)? "" : "X" + stack;
    }

    public void ChangeSlotOpacity(float precentage)
    {
        Color color = (_active)? _activeColor : _inactiveColor;
        color.a *= precentage;
        _image.color = color;

        _itemName.color = _itemTextColor * precentage;
        _stackNumber.color = _stackTextColor * precentage;
    }

}
