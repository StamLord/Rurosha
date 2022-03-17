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

    public bool isGrounded {get {return CharacterStateMachine.IsGrounded;}}

    public bool isUnder {get {return CharacterStateMachine.IsUnder;}}

    public bool wallDetected {get {return CharacterStateMachine.WallDetected;}}
    public Vector3 wallNormal {get {return CharacterStateMachine.WallNormal;}}
    public float wallAngle {get {return CharacterStateMachine.WallAngle;}}

    public bool ledgeDetected {get {return CharacterStateMachine.LedgeDetected;}}
    public Vector3 ledgePoint {get {return CharacterStateMachine.LedgePoint;}}
}
