using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CursorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _selectionText;
    [SerializeField] private Interactor _interactor;
    [SerializeField] private bool _allCaps;

    void Start()
    {
        _interactor.SelectionChangeEvent += ChangeSelectionText;
    }

    void ChangeSelectionText(string selectionText)
    {
        _selectionText.text = (_allCaps) ? selectionText.ToUpper() : selectionText;
    }
}
