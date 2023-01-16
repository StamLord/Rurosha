using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    [SerializeField] private float maxAbsorbTime = 1f;
    [SerializeField] private float immediateAbsorbRadius = .5f;

    [SerializeField] private List<Modifier> modifiers;

    [SerializeField] private float healthGain = 0f;
    [SerializeField] private float pHealthGain = 0f;
    [SerializeField] private float staminaGain = 10f;
    [SerializeField] private float pStaminaGain = 20f;

    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private GameObject[] gameObjects;
    
    private bool active = true;
    private bool startedCoroutine;

    private void OnTriggerEnter(Collider other) 
    {
        if(active == false || startedCoroutine) return;

        CharacterStats stats = other.transform.root.GetComponent<CharacterStats>();

        if(stats)
            StartCoroutine(BoostCoroutine(stats, other.transform));
    }

    private IEnumerator BoostCoroutine(CharacterStats stats, Transform target)
    {
        startedCoroutine = true;
        float startTime = Time.time;

        while(Time.time - startTime < maxAbsorbTime)
        {
            if(Vector3.Distance(transform.position, target.position) <= immediateAbsorbRadius)
                break;
            
            float p = (Time.time - startTime) / maxAbsorbTime;
            transform.position = Vector3.Lerp(transform.position, target.position + target.up, p);
            yield return null;
        }

        stats.RestoreHealth(healthGain, pHealthGain);
        stats.RestoreStamina(staminaGain, pStaminaGain);
        stats.AddModifiers(modifiers);

        Deactivate();
        startedCoroutine = false;
    }

    private void Deactivate()
    {
        active = false;

        foreach(ParticleSystem p in particleSystems)
            p.enableEmission = false;

        foreach(GameObject g in gameObjects)
            g.SetActive(false);
    }

    private void Activate()
    {
        active = true;
        
        foreach(ParticleSystem p in particleSystems)
            p.enableEmission = true;
        
        foreach(GameObject g in gameObjects)
            g.SetActive(true);
    }
}
