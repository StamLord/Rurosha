using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : StateMachine
{
    [Space(20f)]

    public CharacterStats characterStats;
    public InputState inputState;

    [Header("Stealth Agent")]
    [SerializeField] protected StealthAgent stealthAgent;
    
    public void SetVisibility(float visibility)
    {
        stealthAgent.SetVisibility(visibility);
    }

    public void SetDetection(float detection)
    {
        stealthAgent.SetDetection(detection);
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
