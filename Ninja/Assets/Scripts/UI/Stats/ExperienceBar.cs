using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    [SerializeField] private Image expBar;
    [SerializeField] private Image expBarOutline;

    [SerializeField] private TextMeshProUGUI expAddText;
    [SerializeField] private float expAddFade = 1f;
    [SerializeField] private float expAddDuration = 1f;
    private Coroutine addTextCoroutine;

    [SerializeField] private float autoHideAfter = 3f;
    [SerializeField] private float autoHideFade = 1f;
    private bool hidden;
    private bool autoHidePlaying;
    private Coroutine autoHideCoroutine;
    private float lastShow;

    private void Start() 
    {
        if(stats == null) return;

        stats.OnAddExp += AddExp;
        stats.OnLevelUp += LevelUp;
        UpdateExpBar();

        // Start Hidden
        hidden = true;
        Color barColor = expBar.color;
        Color outlineColor = expBarOutline.color;
        barColor.a = outlineColor.a = 0f;

        expBar.color = barColor;
        expBarOutline.color = outlineColor;
    }

    private void Update() 
    {
        if(hidden == false 
            && autoHidePlaying == false 
            && Time.time - lastShow > autoHideAfter)   
            autoHideCoroutine = StartCoroutine("HideBar");
    }

    private void UpdateExpBar()
    {
        expBar.fillAmount = (float)stats.Experience / stats.NextLevel;
        ShowBar();
    }

    private void AddExp(int amount)
    {
        UpdateExpBar();
        
        if(addTextCoroutine != null)
            StopCoroutine(addTextCoroutine);
        
        addTextCoroutine = StartCoroutine(AddExpText(amount));
    }

    private void LevelUp(int level)
    {
        UpdateExpBar();
    }

    private IEnumerator AddExpText(int amount)
    {
        expAddText.text = "+" + amount;

        float startTime = Time.time;
        Color textColor = expAddText.color;

        // Fade in
        while(Time.time - startTime <= expAddFade)
        {
            textColor.a = Mathf.Lerp(0, 1, (Time.time - startTime) / expAddFade);
            expAddText.color = textColor;
            yield return null;   
        }

        textColor.a = 1;
        expAddText.color = textColor;

        // Stay visible
        startTime = Time.time;
        while(Time.time - startTime <= expAddDuration)
        {
            yield return null;   
        }

        // Fade out
        startTime = Time.time;
        while(Time.time - startTime <= expAddFade)
        {
            textColor.a = Mathf.Lerp(1, 0, (Time.time - startTime) / expAddFade);
            expAddText.color = textColor;
            yield return null;   
        }
        
        textColor.a = 0;
        expAddText.color = textColor;
    }

    private void ShowBar()
    {
        if(autoHidePlaying && autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHidePlaying = false;
        }
        
        lastShow = Time.time;
        hidden = false;

        Color barColor = expBar.color;
        Color outlineColor = expBarOutline.color;
        barColor.a = outlineColor.a = 1f;
        
        expBar.color = barColor;
        expBarOutline.color = outlineColor;
    }

    private IEnumerator HideBar()
    {
        autoHidePlaying = true;
        float startTime = Time.time;
        Color barColor = expBar.color;
        Color outlineColor = expBarOutline.color;

        // Fade out
        while(Time.time - startTime <= autoHideFade)
        {
            float t = (Time.time - startTime) / autoHideFade;
            float alpha = Mathf.Lerp(1, 0, t);

            barColor.a = outlineColor.a = alpha;

            expBar.color = barColor;
            expBarOutline.color = outlineColor;
            
            yield return null;
        }

        barColor.a = outlineColor.a = 0f;
        expBar.color = barColor;
        expBarOutline.color = outlineColor;
        hidden = true;
        autoHidePlaying = false;
    }
}
