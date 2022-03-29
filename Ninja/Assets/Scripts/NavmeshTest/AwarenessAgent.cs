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

    [Header("Hearing")]
    [SerializeField] private float hearingRadius = 20f;
    [SerializeField] private Vector3 lastSoundDetected;

    [Header("Debug")]
    [SerializeField] private bool debugView;
    [SerializeField] private Light debugLight;
    [SerializeField] private Color debugLightBaseColor = Color.white;
    [SerializeField] private Color debugLightDetectedColor = Color.red;

    void Update()
    {
        VisionUpdate();
        DebugLight();
    }

    void VisionUpdate()
    {
        // Get all entities in lookRadius
        Collider[] inRange = Physics.OverlapSphere(transform.position, lookRadius, visionMask);
        List<StealthAgent> stealthAgents = new List<StealthAgent>();

        // Check all relevant entities
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
            if(IsLineOfSight(sAgent))
                AddVisibleTarget(sAgent);
        }
        
        // Find which agents are not visible anymore
        List<StealthAgent> toRemove = new List<StealthAgent>();
        foreach(StealthAgent s in visibleAgents)
        {
            if(stealthAgents.Contains(s) == false) 
                toRemove.Add(s);
            else
            {
                if(IsLineOfSight(s) == false)
                    toRemove.Add(s);
            }
        }

        // Remove the agents not visible anymore
        foreach(StealthAgent s in toRemove)
            visibleAgents.Remove(s);
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
            if(blocked) Debug.Log(hit.transform.name);
            return (blocked == false);
        }

        return false;
    }

    void AddVisibleTarget(StealthAgent sAgent)
    {
        /*foreach(StealthAgent s in visibleAgents)
            if(s == sAgent)
                return;
        */
        if(visibleAgents.Contains(sAgent)) return;
        
        visibleAgents.Add(sAgent);
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
        debugLight.color = (VisibleAgents.Count > 0)? debugLightDetectedColor : debugLightBaseColor;
    }

    void OnDrawGizmosSelected()
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
//float distance = Vector3.Distance(target.position,transform.position);
        
        /*if(distance <= lookRadius)
        {
            Vector3 direction = target.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            //Debug.Log(angle);
            if(angle <= visionAngle)
            {
                RaycastHit hit;
                Physics.Raycast(eyeLevel.position, target.position, out hit, lookRadius);
                //if(hit.transform.parent == target)
                    
            }
        }*/