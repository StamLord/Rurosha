using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    [SerializeField] private TextMeshProUGUI moneyText;

    [SerializeField] private float animateInterval = .1f;
    [SerializeField] private float animateDuration = 1f;

    [SerializeField] private float autoHideAfter = 3f;
    [SerializeField] private float autoHideFade = 1f;

    private bool hidden;
    private bool autoHidePlaying;
    private Coroutine autoHideCoroutine;
    private float lastShow;

    private int lastMoneyValue;
    private bool animatePlaying;
    private Coroutine animateCoroutine;

    private void Start() 
    {
        if(stats == null) return;

        stats.OnMoneyUpdate += UpdateDisplay;
        lastMoneyValue = stats.Money;

        // Start Hidden
        hidden = true;
    }

    private void Update() 
    {
        if( hidden == false 
            && autoHidePlaying == false 
            && Time.time - lastShow > autoHideAfter)   
            autoHideCoroutine = StartCoroutine("HideBar");
    }

    private void UpdateDisplay(int amount)
    {
        if(animatePlaying && animateCoroutine != null)
            StopCoroutine(animateCoroutine);
        
        animateCoroutine = StartCoroutine(AnimateMoneyDisplay(lastMoneyValue, stats.Money));

        lastMoneyValue = stats.Money;

        ShowMoney();
    }

    public void ShowMoney()
    {
        if(autoHidePlaying && autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHidePlaying = false;
        }

        lastShow = Time.time;
        hidden = false;
        
        Color color = moneyText.color;
        color.a = 1f;
        moneyText.color = color;
    }

    private IEnumerator AnimateMoneyDisplay(int oldValue, int newValue)
    {
        animatePlaying = true;

        if(oldValue != newValue)
        {
            float startTime = Time.time;
            float totalDuration = Mathf.Abs(newValue - oldValue) * animateInterval;

            // If change is small, animate by increasing value and waiting an interval
            if(totalDuration < animateDuration)
            {
                int increment = 1;
                
                if(newValue < oldValue) 
                    increment = -1;
                
                int value = oldValue;
                while(value != newValue)
                {
                    value += increment;
                    moneyText.text = "" + value;

                    yield return new WaitForSeconds(animateInterval);
                }
            }
            // If change to big, lerp between values to finish in the required time
            else
            {
                while(Time.time - startTime <= animateDuration)
                {
                    float t = (Time.time - startTime) / animateDuration;
                    int value = Mathf.FloorToInt(Mathf.Lerp(oldValue, newValue, t));

                    moneyText.text = "" + value;
                    yield return null;
                }
            }
        }

        moneyText.text = "" + newValue;
        animatePlaying = false;
    }

    private IEnumerator HideBar()
    {
        autoHidePlaying = true;
        float startTime = Time.time;
        Color textColor = moneyText.color;

        // Fade out
        while(Time.time - startTime <= autoHideFade)
        {
            float t = (Time.time - startTime) / autoHideFade;
            float alpha = Mathf.Lerp(1, 0, t);

            textColor.a = alpha;

            moneyText.color = textColor;
            
            yield return null;
        }

        textColor.a = 0f;
        moneyText.color = textColor;
        hidden = true;
        autoHidePlaying = false;
    }

}
