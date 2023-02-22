using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<IHurtboxResponder> responders = new List<IHurtboxResponder>();
    [SerializeField] private PhysicalMaterial physicalMaterial;
    [SerializeField] private Rigidbody rigidbody;
    public Rigidbody Rigidbody { get {return rigidbody;}}

    [Header("Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color hitColor;
    [SerializeField] private float colorFadeStartDuration = .1f;
    [SerializeField] private float colorFadeEndDuration = .5f;
    private Material material;
    
    public void Start()
    {
        material = meshRenderer?.material;
        if(material) baseColor = material.color;
    }

    // Called by weapon scripts after
    public bool Hit(StealthAgent agent, int softDamage, int hardDamage, Vector3 hitUp, DamageType damageType = DamageType.Blunt)
    {
        // Send hit data to all responders and see if atleast 1 returns true
        bool hit = (responders.Count > 0)? false : true;
        foreach(IHurtboxResponder r in responders)
            if(r.GetHit(agent, softDamage, hardDamage, hitUp, damageType))
                hit = true;

        if(hit)
        {
            // Hit Effects
            if(physicalMaterial)
                physicalMaterial.CollideEffect(transform.position, hardDamage, hitUp, damageType);
        
            // Change hurtbox's material color if hit for testing
            if(debug && material)
                StartCoroutine(ColorChange(colorFadeStartDuration, colorFadeEndDuration));
        }
        
        return hit;
    }

    public void AddResponder(IHurtboxResponder responder)
    {
        if(responders.Contains(responder) == false)
            responders.Add(responder);
    }

    IEnumerator ColorChange(float startFadeTime, float endFadeTime)
    {
        float timePassed = 0;
        float totalTime = startFadeTime + endFadeTime;
        float halfTime = totalTime / 2;
        while(timePassed < totalTime)
        {
            if(timePassed < startFadeTime)
                material.color = Color.Lerp(baseColor, hitColor, timePassed / startFadeTime);
            else
                material.color = Color.Lerp(hitColor, baseColor, (timePassed - startFadeTime) / endFadeTime);
                
            timePassed += Time.deltaTime;
            yield return null;
        }

        material.color = baseColor;
    }
}
