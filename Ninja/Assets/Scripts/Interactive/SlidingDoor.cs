using UnityEngine;
using System.Collections;

public class SlidingDoor : Usable
{
    [SerializeField] private bool open;
    [SerializeField] private float slideDuration = .5f;

    [SerializeField] private Vector3 openPos;
    [SerializeField] private Vector3 closePos;
    
    private bool isAnimating;
    private Coroutine coroutine;

    public override void Use(Interactor interactor)
    {
        // Switch State
        open = !open;

        // Animate thorough coroutine

        if(isAnimating && coroutine != null)
            StopCoroutine(coroutine);

        if(open)
            coroutine = StartCoroutine(Animate(closePos, openPos));
        else
            coroutine = StartCoroutine(Animate(openPos, closePos));
    }

    private IEnumerator Animate(Vector3 startPos, Vector3 endPos)
    {
        isAnimating = true;
        float start = Time.time;

        while(Time.time - start <= slideDuration)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, (Time.time - start) / slideDuration);
            yield return null;
        }

        // Make sure we at our goal
        transform.localPosition = endPos;
        isAnimating = false;
    }
}
