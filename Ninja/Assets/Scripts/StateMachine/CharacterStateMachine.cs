using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : StateMachine
{
    [Space(20f)]

    public CharacterStats characterStats;
    public InputState inputState;
    public ColliderManager colliderManager;
    public Kick kick;
    
    [Space(20f)]

    [Header("States")]
    [SerializeField] private State walk;
    [SerializeField] private State run;
    [SerializeField] private State jump;
    [SerializeField] private State air;
    [SerializeField] private State climb;
    [SerializeField] private State dash;
    [SerializeField] private State wallRun;
    [SerializeField] private State crouch;
    [SerializeField] private State roll;
    [SerializeField] private State sit;

    [Space(20f)]

    [Header("Sound Agent")]
    [SerializeField] private StepSoundAgent stepSoundAgent;

    private static bool isDebug;

    public enum StateName
    {
        WALK,
        RUN,
        JUMP,
        AIR,
        CLIMB,
        DASH,
        WALL_RUN,
        CROUCH,
        ROLL,
        SIT
    };

    public void SwitchState(StateName state)
    {
        switch(state)
        {
            case StateName.WALK:
                SwitchState(walk);
                break;
            case StateName.RUN:
                SwitchState(run);
                break;
            case StateName.JUMP:
                SwitchState(jump);
                break;
            case StateName.AIR:
                SwitchState(air);
                break;
            case StateName.CLIMB:
                SwitchState(climb);
                break;
            case StateName.DASH:
                SwitchState(dash);
                break;
            case StateName.WALL_RUN:
                SwitchState(wallRun);
                break;
            case StateName.CROUCH:
                SwitchState(crouch);
                break;
            case StateName.ROLL:
                SwitchState(roll);
                break;
            case StateName.SIT:
                SwitchState(sit);
                break;
        }
    }
    
    private void Start()
    {
        DebugCommandDatabase.AddCommand(new DebugCommand(
                "debugcharstate", 
                "Sets debug of CharacterStateMachine to true or false", 
                "debugcharstate <1/0>", 
                (string[] parameters) => {
                    switch(parameters[0])
                    {
                        case "0":
                            isDebug = false;
                            return "CharacterStateMachine debug set to False";
                        case "1":
                            isDebug = true;
                            return "CharacterStateMachine debug set to True";
                    }
                    return "Parameter should be 1 or 0";
                }));
        
        SwitchState(defaultState);
    }

    public void SetStepSoundAgent(bool active)
    {
        stepSoundAgent.SetActive(active);
    }

    [Header("Stealth Agent")]
    [SerializeField] private StealthAgent stealthAgent;
    
    public void SetVisibility(float visibility)
    {
        if(stealthAgent) stealthAgent.SetVisibility(visibility);
    }

    public void SetDetection(float detection)
    {
        if(stealthAgent) stealthAgent.SetDetection(detection);
    }

    public void SetSoundType(StealthAgent.SoundType type)
    {
        if(stealthAgent) stealthAgent.SetSoundType(type);
    }

    [Header("Ground Detection")]
    [SerializeField] protected GroundSensor groundSensor;
    public bool IsGrounded { get {return groundSensor.IsGrounded;}}
    public float GroundSlope { get {return groundSensor.GroundSlope;}}
    public Vector3 GroundNormal { get {return groundSensor.GroundNormal;}}

    [Header("Ceiling Detection")]
    [SerializeField] protected GroundSensor ceilSensor;
    public bool IsUnder { get {return ceilSensor.IsGrounded;}}

    [Header("Wall Collision Detection")]
    [SerializeField] protected WallSensor wallSensor;
    public bool WallDetected { get {return wallSensor.WallDetected;}}
    public Vector3 WallNormal{ get {return wallSensor.WallNormal;}}
    public Vector3 WallPoint{ get {return wallSensor.WallPoint;}}
    public float WallAngle{ get {return wallSensor.WallAngle;}}
    public bool WallDetect(Vector3 direction, out Vector3 point, out Vector3 normal, out float angle)
    {
        return wallSensor.DetectWall(direction, out point, out normal, out angle);
    }

    [Header("Ledge Detection")]
    [SerializeField] protected LedgeSensor ledgeSensor;
    public bool LedgeDetected { get {return ledgeSensor.LedgeDetected;}}
    public Vector3 LedgePoint { get {return ledgeSensor.LedgePoint;}}

    private void OnGUI() 
    {
        if(isDebug == false) return;
        
        Camera cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        if(viewportPos.x <= 0 || viewportPos.x >= 1 || viewportPos.y <= 0 || viewportPos.y >= 1 || viewportPos.z < 0) return;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(screenPos.x, screenPos.y, 100, 50), CurrentState);
    }
}
