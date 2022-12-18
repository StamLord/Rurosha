using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] _head;
    [SerializeField] private GameObject[] _top;
    [SerializeField] private GameObject[] _bottom;

    [Header("Appearance")]
    [Range(-1,10)][SerializeField] private int _headIndex;
    [Range(-1,10)][SerializeField] private int _topIndex;
    [Range(-1,10)][SerializeField] private int _bottomIndex;

    [SerializeField] private int _lastHeadIndex = -1;
    [SerializeField] private int _lastTopIndex = -1;
    [SerializeField] private int _lastBottomIndex = -1;

    private void OnValidate() 
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        UpdateCollection(_headIndex, ref _lastHeadIndex, _head);
        UpdateCollection(_topIndex, ref _lastTopIndex, _top);
        UpdateCollection(_bottomIndex, ref _lastBottomIndex, _bottom);
    }

    private void UpdateCollection(int index, ref int lastIndex, GameObject[] collection)
    {
        if(index == lastIndex) return;
        
        if(lastIndex >= 0 && lastIndex < collection.Length) 
            collection[lastIndex].SetActive(false);
        
        if(index >= 0 && index < collection.Length) 
            collection[index].SetActive(true);
        lastIndex = index;
    }

    public void SetAppearance(int head, int top, int bottom)
    {
        _headIndex = head;
        _topIndex = top;
        _bottomIndex = bottom;

        UpdateVisuals();
    }
}