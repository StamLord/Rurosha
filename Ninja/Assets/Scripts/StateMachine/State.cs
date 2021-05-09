using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    [SerializeField] protected StateMachine _stateMachine;
    [SerializeField] protected bool isActive;
    public bool IsActive{get{return isActive;}}

    public void EnterState(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        isActive = true;
        OnEnterState();
    }

    public void ExitState()
    {
        isActive = false;
        OnExitState();
    }

    protected virtual void OnEnterState()
    {
        
    }

    public virtual void OnStateUpdate()
    {

    }

    protected virtual void OnExitState()
    {

    }
}
