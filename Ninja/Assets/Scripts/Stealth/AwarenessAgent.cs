using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessAgent : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Vision")]
    [SerializeField] private Transform eyeLevel;
    [SerializeField] private float lookRadius = 10f;
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private LayerMask blockVisionMask;
    [SerializeField] private List<StealthAgent> visibleAgents = new List<StealthAgent>();
    [SerializeField] public List<StealthAgent> VisibleAgents {get {return visibleAgents;}}

    [SerializeField] public List<AlmostVisible> almostVisibleAgents = new List<AlmostVisible>();
    [SerializeField] public float detectRate = 1f;
    [SerializeField] public float undetectRate = .3f;

    [Header("Hearing")]
    [SerializeField] private float hearingRadius = 20f;
    [SerializeField] private Vector3 lastSoundDetected;

    [Header("Debug")]
    [SerializeField] private bool debugView;
    [SerializeField] private Light debugLight;
    [SerializeField] private Color debugLightBaseColor = Color.white;
    [SerializeField] private Color debugLightAlmostColor = Color.yellow;
    [SerializeField] private Color debugLightDetectedColor = Color.red;
    
    [System.Serializable]
    public class AlmostVisible
    {
        public StealthAgent stealthAgent;
        public float timeNoticed;
        public float detected;
    }

    private void Update()
    {
        VisionUpdate();
        DebugLight();
    }

    private void VisionUpdate()
    {
        // Get all entities in lookRadius
        Collider[] inRange = Physics.OverlapSphere(transform.position, lookRadius, visionMask);
        List<StealthAgent> stealthAgents = new List<StealthAgent>();

        // Get StealthAgents
        foreach(Collider col in inRange)
        {
            if(col.transform != transform)
            {   
                StealthAgent sAgent = col.transform.root.GetComponent<StealthAgent>();
                if(sAgent) stealthAgents.Add(sAgent);
            }
        }

        // Check visibility
        foreach(StealthAgent sAgent in stealthAgents)
        {   
            // No need to do anytihng if already in visibleAgents list
            if(visibleAgents.Contains(sAgent)) 
                continue;
                
            if(IsLineOfSight(sAgent))
                AddAlmostVisible(sAgent);
        }

        // Loop over "almost visible" agent
        List<AlmostVisible> toRemoveAv = new List<AlmostVisible>();

        for(int i = 0; i < almostVisibleAgents.Count; i++)
        {
            AlmostVisible av = almostVisibleAgents[i];
            
            // If in line of sight, we add to detection until it's 1.
            // Otherwise, we substract from detection until it's 0
            // We inverse detection modfier to "undetect" so if in crouch state player is detected slower, he will also be "undetected" faster
            if(IsLineOfSight(av.stealthAgent))
                av.detected += (detectRate * av.stealthAgent.Detection * Time.deltaTime);
            else
                av.detected -= (undetectRate * Time.deltaTime);
            
            // Update stealthAgent of this value
            av.stealthAgent.SetAwareness(this, av.detected);

            if(av.detected >= 1)
            {
                AddVisible(av.stealthAgent);
                toRemoveAv.Add(av);
            }
            else if (av.detected <= 0)
            {
                toRemoveAv.Add(av);
                av.stealthAgent.RemoveAwareness(this);
            }
        }

        // Remove the agents not visible anymore
        foreach(AlmostVisible av in toRemoveAv)
            RemoveAlmostVisible(av);
        
        // Find which agents are not visible anymore
        List<StealthAgent> toRemove = new List<StealthAgent>();
        foreach(StealthAgent s in visibleAgents)
        {
            if(IsLineOfSight(s) == false)
                toRemove.Add(s);
        }

        // Remove the agents not visible anymore
        foreach(StealthAgent s in toRemove)
        {
            s.RemoveAwareness(this);
            RemoveVisible(s);
        }
    }

    private bool IsLineOfSight(StealthAgent sAgent)
    {   
        Vector3 direction = sAgent.EyeLevelPosition - eyeLevel.position;

        // Check visibility modifier
        // Example 1: lookRadius is 10 and visibilit is 1 - Player will be visibile from 10 meters away
        // Example 2: lookRadius is 10 and visibilit is .7 - Player will be visibile from 7 meters away
        if(direction.magnitude > lookRadius * sAgent.Visibility) return false;

        float angle = Vector3.Angle(transform.forward, direction);
        if(angle <= visionAngle)
        {    
            // Check line of sight
            RaycastHit hit;
            Debug.DrawRay(eyeLevel.position, direction, Color.yellow);
            bool blocked = Physics.Raycast(eyeLevel.position, direction, out hit, direction.magnitude, blockVisionMask);
            return (blocked == false);
        }

        return false;
    }

    private void AddAlmostVisible(StealthAgent sAgent)
    {
        // Make sure we don't add duplicates
        foreach(AlmostVisible av in almostVisibleAgents)
            if (av.stealthAgent == sAgent) return;
        
        // Add to list
        AlmostVisible newAlmostVisible = new AlmostVisible();
        newAlmostVisible.timeNoticed = Time.time;
        newAlmostVisible.stealthAgent = sAgent;
        almostVisibleAgents.Add(newAlmostVisible);
    }

    private void RemoveAlmostVisible(AlmostVisible almostVisible)
    {
        almostVisibleAgents.Remove(almostVisible);
    }

    private void AddVisible(StealthAgent sAgent)
    {
        if(visibleAgents.Contains(sAgent)) return;
        visibleAgents.Add(sAgent);
    }

    private void RemoveVisible(StealthAgent sAgent)
    {
        visibleAgents.Remove(sAgent);
    }

    public void AddSound(Vector3 soundOrigin)
    {
        lastSoundDetected = soundOrigin;
    }

    private void DebugLight()
    {
        if(debugView == false || debugLight == null) return;

        debugLight.range = lookRadius;
        debugLight.innerSpotAngle = debugLight.spotAngle = visionAngle * 2;

        if(VisibleAgents.Count > 0)
            debugLight.color = debugLightDetectedColor;
        else if(almostVisibleAgents.Count == 0)
            debugLight.color = debugLightBaseColor;
        else
        {   
            // Find most detected target 
            float detect = Mathf.Infinity;
            foreach(AlmostVisible av in almostVisibleAgents)
            {
                if(av.detected < detect)
                    detect = av.detected;
            }

            debugLight.color = Color.Lerp(debugLightBaseColor, debugLightAlmostColor, detect);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(debugView == false) return;
        
        // Vision Cone Representation
        Color coneColor = Color.red;
        Color radiusColor = Color.red;
        radiusColor.a = .5f;
        
        Gizmos.color = radiusColor;
        Gizmos.DrawWireSphere(transform.position, lookRadius);

        Gizmos.color = coneColor;
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + Quaternion.AngleAxis(visionAngle, Vector3.up) * eyeLevel.forward * lookRadius);
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + Quaternion.AngleAxis(-visionAngle, Vector3.up) * eyeLevel.forward * lookRadius);
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + Quaternion.AngleAxis(visionAngle, Vector3.right) * eyeLevel.forward * lookRadius);
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + Quaternion.AngleAxis(-visionAngle, Vector3.right) * eyeLevel.forward * lookRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + eyeLevel.forward);

        // Visible Agents Representation
        Gizmos.color = Color.yellow;
        foreach(StealthAgent s in visibleAgents)
            Gizmos.DrawCube(s.transform.position, new Vector3(1f, 1f, 1f));

        // Last Sound Representation
        Gizmos.color = Color.white;   
        Gizmos.DrawCube(lastSoundDetected, new Vector3(1f, 1f, 1f));

    }
}