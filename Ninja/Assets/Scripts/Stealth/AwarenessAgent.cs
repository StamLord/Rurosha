using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessAgent : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private Transform eyeLevel;
    [SerializeField] private float lookRadius = 10f;
    [SerializeField] private float horizontalVisionAngle = 45f;
    [SerializeField] private float verticalVisionAngle = 20f;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private LayerMask blockVisionMask;
    [SerializeField] private List<StealthAgent> visibleAgents = new List<StealthAgent>();
    [SerializeField] public List<StealthAgent> VisibleAgents {get {return visibleAgents;}}

    [SerializeField] public List<AlmostVisible> almostVisibleAgents = new List<AlmostVisible>();
    [SerializeField] public float detectRate = 1f;
    [SerializeField] public float undetectRate = .3f;

    [SerializeField] public bool alert;
    [SerializeField] public float alertDetectRate = 10f;
    [SerializeField] public float alertUndetectRate = .1f;

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

    private List<StealthAgent> agentsInRange = new List<StealthAgent>();
    private List<AlmostVisible> almostVisibleToRemove= new List<AlmostVisible>();
    private List<StealthAgent> agentsToRemove= new List<StealthAgent>();

    public delegate void onSeeAgentDelegate(StealthAgent agent);
    public event onSeeAgentDelegate OnSeeAgent;

    public delegate void onLoseAgentDelegate(StealthAgent agent);
    public event onLoseAgentDelegate OnLoseAgent;

    public delegate void onHearSoundDelegate(Vector3 position);
    public event onHearSoundDelegate OnHearSound;

    private void Update()
    {
        VisionUpdate();
        //DebugLight();
    }

    public void SetAlert(bool state)
    {
        alert = state;
    }

    private void VisionUpdate()
    {
        // Get all entities in lookRadius
        Collider[] inRange = Physics.OverlapSphere(transform.position, lookRadius, visionMask);
        agentsInRange.Clear();

        // Get StealthAgents
        foreach(Collider col in inRange)
        {
            if(col.transform != transform)
            {   
                StealthAgent sAgent = col.transform.root.GetComponent<StealthAgent>();
                if(sAgent) agentsInRange.Add(sAgent);
            }
        }
        
        // Check visibility
        foreach(StealthAgent sAgent in agentsInRange)
        {   
            // No need to do anytihng if already in visibleAgents list
            if(visibleAgents.Contains(sAgent)) 
                continue;
                
            if(IsLineOfSight(sAgent))
                AddAlmostVisible(sAgent);
        }

        // Loop over "almost visible" agent
        almostVisibleToRemove.Clear();
        
        float detect = detectRate;
        float undetect = undetectRate;
        if(alert)
        {
            detect = alertDetectRate;
            undetect = alertUndetectRate;
        }

        for(int i = 0; i < almostVisibleAgents.Count; i++)
        {
            AlmostVisible av = almostVisibleAgents[i];
            
            // If in line of sight, we add to detection until it's 1.
            // Otherwise, we substract from detection until it's 0
            // We inverse detection modfier to "undetect" so if in crouch state player is detected slower, he will also be "undetected" faster
            if(IsLineOfSight(av.stealthAgent))
                av.detected += (detect * av.stealthAgent.Detection * Time.deltaTime);
            else
                av.detected -= (undetect * Time.deltaTime);
            
            // Update stealthAgent of this value
            av.stealthAgent.SetAwareness(this, av.detected);

            if(av.detected >= 1)
            {
                AddVisible(av.stealthAgent);
                almostVisibleToRemove.Add(av);
            }
            else if (av.detected <= 0)
            {
                almostVisibleToRemove.Add(av);
                av.stealthAgent.RemoveAwareness(this);
            }
        }

        // Remove the agents not visible anymore
        foreach(AlmostVisible av in almostVisibleToRemove)
            RemoveAlmostVisible(av);
        
        // Find which agents are not visible anymore
        agentsToRemove.Clear();
        foreach(StealthAgent s in visibleAgents)
        {
            if(IsLineOfSight(s) == false)
                agentsToRemove.Add(s);
        }

        // Remove the agents not visible anymore
        foreach(StealthAgent s in agentsToRemove)
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
        
        // Check angle

        // Simple angle used for both vertical and horizontal view
        // float angle = Vector3.Angle(transform.forward, direction);
        // if(angle > visionAngle)
        //     return false;


        // Split angles for vertical / horizontal

        // Get angle around X axis - Angle between direction to target and a "flattned" direction with y component = 0
        Vector3 flatYDir = new Vector3(direction.x, 0, direction.z);
        float angleX = Vector3.Angle(direction, flatYDir);
        
        if(angleX > verticalVisionAngle) return false;

        // Get angle around Y axis - We flatten both us and target on y axis ( y = 0) and get angle between them
        Vector3 tPos = new Vector3(sAgent.transform.position.x, 0, sAgent.transform.position.z);
        Vector3 lPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 dir2 = tPos - lPos;
        float angleY = Vector3.Angle(transform.forward, dir2);

        if(angleY > horizontalVisionAngle) return false;

        // Check line of sight
        RaycastHit hit;
        Debug.DrawRay(eyeLevel.position, direction, Color.yellow);
        bool blocked = Physics.Raycast(eyeLevel.position, direction, out hit, direction.magnitude, blockVisionMask);
        return (blocked == false);
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
        
        if(OnSeeAgent != null)
            OnSeeAgent(sAgent);
    }

    private void RemoveVisible(StealthAgent sAgent)
    {
        visibleAgents.Remove(sAgent);

        if(OnLoseAgent != null)
            OnLoseAgent(sAgent);
    }

    public void AddSound(Vector3 soundOrigin)
    {
        lastSoundDetected = soundOrigin;
        if(OnHearSound != null)
            OnHearSound(soundOrigin);
    }

    private void DebugLight()
    {
        if(debugView == false || debugLight == null) return;

        debugLight.range = lookRadius;
        debugLight.innerSpotAngle = debugLight.spotAngle = verticalVisionAngle * 2;

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
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + Quaternion.AngleAxis(horizontalVisionAngle, transform.up) * eyeLevel.forward * lookRadius);
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + Quaternion.AngleAxis(-horizontalVisionAngle, transform.up) * eyeLevel.forward * lookRadius);
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + Quaternion.AngleAxis(verticalVisionAngle, transform.right) * eyeLevel.forward * lookRadius);
        Gizmos.DrawLine(eyeLevel.position, eyeLevel.position + Quaternion.AngleAxis(-verticalVisionAngle, transform.right) * eyeLevel.forward * lookRadius);

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