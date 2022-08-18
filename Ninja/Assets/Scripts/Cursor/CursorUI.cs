using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CursorUI : MonoBehaviour
{
    [Header("Cursors")]
    [SerializeField] private Image cursorImage;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Sprite cursorSingle; 
    [SerializeField] private Sprite cursorTripleHorizontal; 
    [SerializeField] private Sprite cursorTripleVetical;
    [SerializeField] private Sprite cursorDiagonalCross;

    [Header("Selection Text")]
    [SerializeField] private TextMeshProUGUI selectionText;
    [SerializeField] private Interactor interactor;
    [SerializeField] private bool allCaps;

    private void Start()
    {
        interactor.SelectionChangeEvent += ChangeSelectionText;
        weaponManager.ChangeCursorEvent += ChangeCursor;
    }

    private void ChangeSelectionText(string selectionText)
    {
        this.selectionText.text = (allCaps) ? selectionText.ToUpper() : selectionText;
    }

    private void ChangeCursor(CursorType cursor)
    {
        Sprite s = null;
        switch(cursor)
        {
            case CursorType.SINGLE:
                s= cursorSingle;
                break;
            case CursorType.HORIZONTAL:
                s = cursorTripleHorizontal;
                break;
            case CursorType.VERTICAL:
                s = cursorTripleVetical;
                break;
            case CursorType.DIAGONAL_CROSS:
                s = cursorDiagonalCross;
                break;
        }

        cursorImage.sprite = s;
        cursorImage.SetNativeSize();
    }
}
