﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _outline;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _stackNumber;

    [SerializeField] private float _stackUpdateAnimationDuration = 1f;
    [SerializeField] private Vector3 _stackUpdateAnimationSize = new Vector3(2f, 2f, 2f);
    private bool isAnimatingStack;

    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _activeOutlineColor;

    private Color _itemTextColor;
    private Color _stackTextColor;
    private Color _inactiveColor;
    private Color _inactiveOutline;
    

    [SerializeField] private bool _active;

    void Start()
    {
        _itemTextColor = _itemName.color;
        _stackTextColor = _stackNumber.color;
        _inactiveColor = _image.color;
        _inactiveOutline = _outline.color;
    }

    public void SetActive(bool active)
    {
        _active = active;
    }

    public void UpdateItem(Item item, int stack)
    {
        _itemName.text = (item != null)? item.itemName : "";
        _stackNumber.text = (stack == 0)? "" : "X" + stack;
        if(isAnimatingStack == false) 
            StartCoroutine("StackAnimation");
    }

    public void ChangeSlotOpacity(float precentage)
    {
        Color color = (_active)? _activeColor : _inactiveColor;
        color.a *= precentage;
        _image.color = color;
        
        color = (_active) ? _activeOutlineColor : _inactiveOutline;
        color.a *= precentage;
        _outline.color = color;

        _itemName.color = _itemTextColor * precentage;
        _stackNumber.color = _stackTextColor * precentage;
    }

    IEnumerator StackAnimation()
    {
        isAnimatingStack = true;
        float time = Time.time;
        while(Time.time < time + _stackUpdateAnimationDuration)
        {
            float p = Time.time / (time + _stackUpdateAnimationDuration);
            _stackNumber.rectTransform.localScale = Vector3.Lerp(_stackUpdateAnimationSize, Vector3.one, p);
            yield return null;
        }
        isAnimatingStack = false;
    }

}