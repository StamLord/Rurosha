using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitWindow : UIWindow
{
    [SerializeField] private TimeSkipper skipper;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderValue;

    public void Skip()
    {
        if(skipper.TimeSkip(Mathf.RoundToInt(slider.value), 0, 0))
            Close();
    }

    public void UpdateValue()
    {
        sliderValue.text = slider.value + " HOURS";
    }
}
