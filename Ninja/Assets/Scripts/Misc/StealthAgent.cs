using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthAgent : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private float _visibility;

    [Header("Sound")]
    [SerializeField] private float walkSound = 3f;
    [SerializeField] private float runSound = 5f;
    [SerializeField] private float crouchSound = .5f;
    [SerializeField] private LayerMask soundMask;
    
    [SerializeField] private float lastSound;
    [SerializeField] private float soundEvery = 1f;

    public float visibility { get{return _visibility; } }

    void Start()
    {
        
    }

    void Update()
    {
        if(Time.time - lastSound >= soundEvery)
        {    
            CreateSound(walkSound);
            lastSound = Time.time;
        }
    }

    void CreateSound(float radius)
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, radius, soundMask);
        
        foreach(Collider col in inRange)
        {
            if(col.transform != transform)
            {   
                AwarenessAgent aAgent = col.transform.GetComponent<AwarenessAgent>();
                if(aAgent) aAgent.AddSound(transform.position);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Color soundColor = Color.black;
        soundColor.a = .5f;

        Gizmos.color = soundColor;
        Gizmos.DrawWireSphere(transform.position, walkSound);
    }

}
