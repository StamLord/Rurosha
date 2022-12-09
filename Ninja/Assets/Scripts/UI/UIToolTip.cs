using UnityEngine;
using TMPro;

public class UIToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI toolTip;

    public void UpdateText(string text)
    {
        toolTip.text = text;
    }
}
