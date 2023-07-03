using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI popupText;

    [SerializeField] private string suffix;
    [SerializeField] private string prefix;
    
    [SerializeField] private float popupDuration = 3f;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;

    private bool popupPlaying;
    private Coroutine popupCoroutine;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Override to subscribe StartPopup(int amount) to different events
    /// </summary>
    protected virtual void Initialize()
    {

    }

    protected void StartPopup(int amount)
    {
        if(popupPlaying && popupCoroutine != null)
            StopCoroutine(popupCoroutine);
        
        popupCoroutine = StartCoroutine(Popup(amount));

    }

    protected IEnumerator Popup(int amount)
    {
        popupPlaying = true;

        // Build string
        string text = prefix;

        if(amount < 0)
            text += " ";
        else
            text += " +";

        text += amount + suffix;

        popupText.text = text;

        float startTime = Time.time;
        Color textColor = popupText.color;

        // Fade in
        while(Time.time - startTime <= fadeInDuration)
        {
            textColor.a = Mathf.Lerp(0, 1, (Time.time - startTime) / fadeInDuration);
            popupText.color = textColor;
            yield return null;   
        }

        textColor.a = 1;
        popupText.color = textColor;

        // Stay visible
        startTime = Time.time;
        while(Time.time - startTime <= popupDuration)
        {
            yield return null;   
        }

        // Fade out
        startTime = Time.time;
        while(Time.time - startTime <= fadeOutDuration)
        {
            textColor.a = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeOutDuration);
            popupText.color = textColor;
            yield return null;   
        }
        
        textColor.a = 0;
        popupText.color = textColor;

        popupPlaying = true;
    }
}
