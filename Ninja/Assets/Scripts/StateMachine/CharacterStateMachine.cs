using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : StateMachine
{
    public CharacterStats characterStats;
    public InputState inputState;

    [Header("Stealth Agent")]
    [SerializeField] protected StealthAgent stealthAgent;
    public void SetVisibility(float visibility)
    {
        stealthAgent.SetVisibility(visibility);
    }

    [Header("Ground Detection")]
    [SerializeField] protected GroundSensor groundSensor;
    public bool IsGrounded { get {return groundSensor.IsGrounded;}}

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
