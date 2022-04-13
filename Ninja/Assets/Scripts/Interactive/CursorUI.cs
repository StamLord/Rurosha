using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CursorUI : MonoBehaviour
{
    [Header("Selection")]
    [SerializeField] private TextMeshProUGUI _selectionText;
    [SerializeField] private Interactor _interactor;
    [SerializeField] private bool _allCaps;

    private void Start()
    {
        _interactor.SelectionChangeEvent += ChangeSelectionText;
    }

    private void ChangeSelectionText(string selectionText)
    {
        _selectionText.text = (_allCaps) ? selectionText.ToUpper() : selectionText;
    }
}
