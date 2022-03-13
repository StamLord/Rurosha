using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlideEnabler : MonoBehaviour
{
    [SerializeField] private AirState airState;
    [SerializeField] private Switch _switch;
    [SerializeField] private TextMeshProUGUI enabledText;

    private void Start()
    {
        _switch.StateChangeEvent += EnableGlide;
    }

    public void EnableGlide(bool state)
    {
        airState.glideOn = state;
        enabledText.text = "Glide: ";
        enabledText.text += (state) ? "ENABLED" : "DISABLED";
    }
}
