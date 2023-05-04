using UnityEngine;
using System.Collections;

public class SlidingDoor : Usable
{
    [SerializeField] private bool open;
    [SerializeField] private float slideDuration = .5f;

    [SerializeField] private Transform openPos;
    [SerializeField] private Transform closePos;
    
    private bool isAnimating;
    private Coroutine coroutine;

    private MeshFilter meshFilter;

    public override void Use(Interactor interactor)
    {
        // Switch State
        open = !open;

        // Animate thorough coroutine

        if(isAnimating && coroutine != null)
            StopCoroutine(coroutine);

        if(open)
            coroutine = StartCoroutine(Animate(openPos.position));
        else
            coroutine = StartCoroutine(Animate(closePos.position));
    }

    private IEnumerator Animate(Vector3 endPos)
    {
        isAnimating = true;
        float start = Time.time;
        Vector3 startPos = transform.position;

        while(Time.time - start <= slideDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (Time.time - start) / slideDuration);
            yield return null;
        }

        // Make sure we at our goal
        transform.position = endPos;
        isAnimating = false;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(openPos.position, .2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(closePos.position, .2f);
    }
}
