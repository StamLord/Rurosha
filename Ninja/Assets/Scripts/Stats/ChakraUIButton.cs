using UnityEngine;
using UnityEngine.UI;

public class ChakraUIButton : MonoBehaviour
{
    [SerializeField] private ChakraUIWindow chakraUIWindow;
    [SerializeField] private ChakraType type;
    [SerializeField] private Image image;

    public ChakraType Type { get {return type;}}

    public void Select()
    {
        chakraUIWindow.Select(transform.position, type);
    }

    public void UpdateValue(float amount)
    {
        image.fillAmount = amount;
    }
}
