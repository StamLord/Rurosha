using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChakraConvertWindow : UIWindow
{
    [SerializeField] private Image fromIcon;
    [SerializeField] private Image toIcon;

    [SerializeField] private Image fromBar;
    [SerializeField] private Image toBar;
    [SerializeField] private Image toBarUpdated;

    [SerializeField] private TextMeshProUGUI fromText;
    [SerializeField] private TextMeshProUGUI toText;

    [SerializeField] private Slider slider;

    private ChakraUIWindow chakraUIWindow;
    private ChakraType fromType;
    private ChakraType toType;
    private float fromAmount;
    private float toAmount;
    private float rate = 1;
    private float minAmount;
    private float maxAmount;
    
    public void Initialize(ChakraUIWindow window, ChakraType fromType, ChakraType toType, float fromAmount, float toAmount, float rate)
    {
        chakraUIWindow = window;
        // Initial values
        this.fromType = fromType;
        this.toType = toType;
        this.fromAmount = fromAmount;
        this.toAmount = toAmount;
        this.rate = rate;

        // Update visual bars
        fromBar.fillAmount = fromAmount;
        toBar.fillAmount = toAmount;
        toBarUpdated.fillAmount = toAmount;

        // Calculate slider limits
        minAmount = toAmount;
        maxAmount = toAmount + fromAmount * rate;

        UpdateText();
        LimitSlider();
    }

    public void ValueUpdate()
    {
        LimitSlider();
        
        // Remap value to our min-max range
        float value = (slider.value - minAmount) / (maxAmount - minAmount);

        // Update visual bars
        fromBar.fillAmount = fromAmount - value * fromAmount;
        toBar.fillAmount = toAmount + value * fromAmount * rate;

        UpdateText();
    }

    private void UpdateText()
    {
        fromText.text = "" + Mathf.RoundToInt(fromBar.fillAmount * 100);
        toText.text = "" + Mathf.RoundToInt(toBar.fillAmount * 100);
    }   

    private void LimitSlider()
    {
        // Clamp to min max values
        slider.value = Mathf.Clamp(slider.value, minAmount, maxAmount);
        
        // Round value to 2 decimals: 1.05, 2.50, etc
        slider.value = (float)decimal.Round((decimal)slider.value, 2);
    }

    public void Convert()
    {
        chakraUIWindow.ApplyConvert(fromType, toType, slider.value * fromAmount);
        Close();
    }
}
