using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitScreenFX : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Image image;
    
    private Coroutine coroutine;

    public void Play()
    {
        if(coroutine != null)
            StopCoroutine(coroutine);
        
        coroutine = StartCoroutine("PlayCoroutine");
    }

    private IEnumerator PlayCoroutine()
    {
        float timeStarted = Time.time;
        Color imageColor = image.color;

        while(Time.time - timeStarted <= duration)
        {
            float p = (Time.time - timeStarted) / duration;
            imageColor.a = 1 - p;
            image.color =  imageColor;

            yield return null;
        }

        imageColor.a = 0;
        image.color =  imageColor;
    }
}
