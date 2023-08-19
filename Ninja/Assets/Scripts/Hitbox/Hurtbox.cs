using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour, IHeatConductor
{
    [Header("References")]
    [SerializeField] private List<IHurtboxResponder> responders = new List<IHurtboxResponder>();
    [SerializeField] private PhysicalMaterial physicalMaterial;

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
        if(meshRenderer) material = meshRenderer.material;
        if(material) baseColor = material.color;
    }

    public bool Hit(AttackInfo attackInfo)
    {
        return Hit(null, attackInfo, Vector3.up, Vector3.zero);
    }

    // Called by weapon scripts after
    public bool Hit(StealthAgent agent, int softDamage, int hardDamage, Vector3 hitUp, Vector3 force, DamageType damageType = DamageType.Blunt, Status[] statuses = null)
    {
        bool hit = false;
        
        // Send hit data to all responders and see if atleast 1 returns true
        foreach(IHurtboxResponder r in responders)
            if(r.GetHit(agent, softDamage, hardDamage, hitUp, force, damageType, statuses))
                hit = true;
        

        if(hit || responders.Count == 0) // Either a succesfull hit on one of the responders or if we have no responders (so no one can guard)
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


    public bool Hit(StealthAgent agent, AttackInfo attackInfo, Vector3 hitUp, Vector3 force)
    {
        return Hit(agent, attackInfo.softDamage, attackInfo.hardDamage, hitUp, force, attackInfo.damageType, attackInfo.statuses);
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

    public void Conduct(float temperature)
    {
        HeatDamage(temperature);
    }

    public bool HeatDamage(float temperature)
    {
        bool hit = false;

        foreach(IHurtboxResponder r in responders)
        {
            if(r.GetHeatDamage(temperature))
                hit = true;
        }

        return hit;
    }
}
