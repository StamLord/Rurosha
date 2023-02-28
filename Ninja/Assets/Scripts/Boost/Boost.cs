using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    [SerializeField] private float unabsorbableTime = 1f;

    [SerializeField] private float maxAbsorbTime = 1f;
    [SerializeField] private float immediateAbsorbRadius = .5f;

    [SerializeField] private List<Modifier> modifiers;

    [SerializeField] private int expGain = 10;

    [SerializeField] private float healthGain = 0f;
    [SerializeField] private float pHealthGain = 0f;
    [SerializeField] private float staminaGain = 10f;
    [SerializeField] private float pStaminaGain = 20f;

    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private GameObject[] gameObjects;
    
    private float birthTime;
    private bool active = true;
    private bool startedCoroutine;

    private void Start() 
    {
        birthTime = Time.time;
    }

    private void OnTriggerStay(Collider other) 
    {
        // Time we can't absorb it for when the boost spawns
        if(Time.time - birthTime < unabsorbableTime) return;

        // Inactive or mid absorbtion
        if(active == false || startedCoroutine) return;

        CharacterStats stats = other.transform.root.GetComponent<CharacterStats>();

        if(stats && stats.gameObject.name == "Player Object (Main)")
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

        stats.AddExp(expGain);

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
        {
            if(p) 
                p.enableEmission = false;
        }

        foreach(GameObject g in gameObjects)
        {
            if(g)
                g.SetActive(false);
        }
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
