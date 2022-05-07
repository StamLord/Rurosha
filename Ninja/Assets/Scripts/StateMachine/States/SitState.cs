using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitState : PlayerState
{
    [Header("Collider References")]
    [SerializeField] private CapsuleCollider standCollider;
    [SerializeField] private CapsuleCollider sitCollider;

    [Header("Debug View")]
    [SerializeField] private bool debugView;

    public delegate void SitStartDelegate();
    public event SitStartDelegate OnSitStart;

    public delegate void SitEndDelegate();
    public event SitEndDelegate OnSitEnd;

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Sit State]");
        
        sitCollider.enabled = true;
        standCollider.enabled = false;
        characterStats.SetSit(true);

        if(OnSitStart != null) OnSitStart();
    }

    protected override void OnExitState()
    {
        base.OnExitState();
        sitCollider.enabled = false;
        standCollider.enabled = true;
        characterStats.SetSit(false);

        if(OnSitEnd != null) OnSitEnd();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        // Stand up
        if(inputState.AxisInput.magnitude > 0 ||
            inputState.Jump.Pressed ||
            inputState.Crouch.Pressed)
        {
            _stateMachine.SwitchState(0);
        }
    }
}
