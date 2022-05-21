using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{

    [SerializeField] private CharacterStateMachine characterStateMachine;
    public CharacterStateMachine CharacterStateMachine { 
        get { 
            return (characterStateMachine == null)? characterStateMachine = (CharacterStateMachine)_stateMachine : characterStateMachine; 
            }}

    public InputState inputState { get { return CharacterStateMachine.inputState; }}
    public CharacterStats characterStats { get { return CharacterStateMachine.characterStats; }}

    public void SetVisibility(float visibility)
    {
        CharacterStateMachine.SetVisibility(visibility);
    }

    public void SetDetection(float detection)
    {
        CharacterStateMachine.SetDetection(detection);
    }

    public bool IsGrounded {get {return CharacterStateMachine.IsGrounded;}}
    public float GroundSlope {get {return CharacterStateMachine.GroundSlope;}}
    public Vector3 GroundNormal {get {return CharacterStateMachine.GroundNormal;}}

    public bool isUnder {get {return CharacterStateMachine.IsUnder;}}

    public bool wallDetected {get {return CharacterStateMachine.WallDetected;}}
    public Vector3 wallNormal {get {return CharacterStateMachine.WallNormal;}}
    public float wallAngle {get {return CharacterStateMachine.WallAngle;}}
    public bool WallDetect(Vector3 direction, out Vector3 point, out Vector3 normal, out float angle)
    {
        return CharacterStateMachine.WallDetect(direction, out point, out normal, out angle);
    }

    public bool ledgeDetected {get {return CharacterStateMachine.LedgeDetected;}}
    public Vector3 ledgePoint {get {return CharacterStateMachine.LedgePoint;}}
}
