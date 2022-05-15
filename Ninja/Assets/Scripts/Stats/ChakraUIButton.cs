using UnityEngine;
using UnityEngine.UI;

public class ChakraUIButton : MonoBehaviour
{
    [SerializeField] private ChakraType type;
    [SerializeField] private Image image;
    [SerializeField] private Image focus;

    public ChakraType Type { get {return type;}}
    private ChakraUIWindow chakraUIWindow;

    public void SetContext(ChakraUIWindow window)
    {
        chakraUIWindow = window;
    }

    public void Select()
    {
        chakraUIWindow.Select(transform.position, this);
    }

    public void UpdateValue(float amount)
    {
        image.fillAmount = amount;
    }

    public void ShowFocus(bool state)
    {
        focus.enabled = state;
    }
}
