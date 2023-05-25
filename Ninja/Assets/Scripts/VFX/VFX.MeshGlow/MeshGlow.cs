using System.Collections;
using UnityEngine;

public class MeshGlow : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    private Material material;
    
    private Coroutine coroutine;

    private void Start()
    {
        material = meshRenderer.material;
    }

    public void Glow(float duration)
    {
        Glow(duration, material.color);
    }

    public void Glow(float duration, Color color)
    {
        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(GlowCoroutine(duration, color));
    }

    private IEnumerator GlowCoroutine(float duration, Color color)
    {
        float startTime = Time.time;
        float halfPoint = duration * .5f;
        float time = 0;

        Color oldColor = material.color;
        material.color = color;

        while(time < duration)
        {
            // Fade in
            if(time < halfPoint)
                material.SetFloat("_GlowValue", Mathf.Lerp(0, 1, time / halfPoint));
            // Fade out
            else
                material.SetFloat("_GlowValue", Mathf.Lerp(1, 0, (time - halfPoint) / halfPoint));

            time = Time.time - startTime;
            yield return null;
        }

        material.SetFloat("_GlowValue", 0);
        material.color = oldColor;
    }
}
