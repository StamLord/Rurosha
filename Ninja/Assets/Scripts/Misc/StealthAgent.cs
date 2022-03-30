using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthAgent : MonoBehaviour
{
    [Header("Eye Level")]
    [SerializeField] private Transform eyeLevel;
    [SerializeField] private Vector3 eyeLevelOffset = new Vector3(0, -.1f, 0);
    public Vector3 EyeLevelPosition{get{return eyeLevel.position + eyeLevelOffset;}}

    [Header("Vision")]
    [SerializeField] private float visibility;

    [Header("Sound")]
    [SerializeField] private float walkSound = 3f;
    [SerializeField] private float runSound = 5f;
    [SerializeField] private float crouchSound = .5f;
    [SerializeField] private LayerMask soundMask;
    
    [SerializeField] private float lastSound;
    [SerializeField] private float soundEvery = 1f;

    public float Visibility { get{return visibility;} }

    [Header("Is Seen [DEBUG]")]
    [SerializeField]private float detectedValue;
    public float DetectedValue { get{return detectedValue;}}
    [SerializeField]private Dictionary<AwarenessAgent, float> awareness = new Dictionary<AwarenessAgent, float>();
    public Dictionary<AwarenessAgent, float> Awareness {get {return awareness;}}

    void Update()
    {
        UpdateHighestDetection();

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

    public void SetVisibility(float visibility)
    {
        this.visibility = visibility;
    }

    public void SetAwareness(AwarenessAgent agent, float value)
    {
        awareness[agent] = Mathf.Clamp01(value);
    }

    public void RemoveAwareness(AwarenessAgent agent)
    {
        awareness.Remove(agent);
    }

    public void UpdateHighestDetection()
    {
        detectedValue = 0;

        foreach(var a in awareness)
        {
            if(a.Value > detectedValue)
                detectedValue = a.Value;
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
