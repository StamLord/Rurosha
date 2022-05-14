using UnityEngine;
using UnityEngine.UI;

public class ChakraUIButton : MonoBehaviour
{
    [SerializeField] private ChakraType type;
    [SerializeField] private Image image;

    public ChakraType Type { get {return type;}}
    private ChakraUIWindow chakraUIWindow;

    public void SetContext(ChakraUIWindow window)
    {
        chakraUIWindow = window;
    }

    public void Select()
    {
        chakraUIWindow.Select(transform.position, type);
    }

    public void UpdateValue(float amount)
    {
        image.fillAmount = amount;
    }
}
