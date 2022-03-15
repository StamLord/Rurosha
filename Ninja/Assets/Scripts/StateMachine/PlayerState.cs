using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
    public bool isGrounded {get {return IsGrounded();}}

    public bool isUnder {get {return IsUnder();}}

    public bool wallDetected {get {return WallDetected();}}
    public Vector3 wallNormal {get {return WallNormal();}}

    public bool ledgeDetected {get {return LedgeDetected();}}
    public Vector3 ledgePoint {get {return LedgePoint();}}

    public bool IsGrounded()
    {
        return ((CharacterStateMachine)_stateMachine).IsGrounded;
    }

    public bool IsUnder()
    {
        return ((CharacterStateMachine)_stateMachine).IsUnder;
    }

    public bool WallDetected()
    {
        return ((CharacterStateMachine)_stateMachine).WallDetected;
    }

    public Vector3 WallNormal()
    {
        return ((CharacterStateMachine)_stateMachine).WallNormal;
    }

    public bool LedgeDetected()
    {
        return ((CharacterStateMachine)_stateMachine).LedgeDetected;
    }

    public Vector3 LedgePoint()
    {
        return ((CharacterStateMachine)_stateMachine).LedgePoint;
    }
}
