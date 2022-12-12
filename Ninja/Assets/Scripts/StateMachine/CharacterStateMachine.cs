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
    
    [Header("Sound Agent")]
    [SerializeField] private StepSoundAgent stepSoundAgent;

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
}
