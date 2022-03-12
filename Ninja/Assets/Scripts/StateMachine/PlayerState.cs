using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
    [Header("Ground Detection")]
    [SerializeField] protected GroundSensor groundSensor;
    public bool IsGrounded { get{return groundSensor.IsGrounded;}}
}
