using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateListener : MonoBehaviour
{
    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private string _stateName;
    [SerializeField] private UnityEvent _enterAction;
    [SerializeField] private UnityEvent _exitAction;

    private void Start() 
    {
        if(_stateMachine)
        {
            _stateMachine.OnStateEnter += EnterState;
            _stateMachine.OnStateExit += ExitState;
        }
    }

    private void EnterState(string stateName)
    {Debug.Log("Enter:" + stateName);
        if(stateName == _stateName)
            ExecuteEnter();
    }

    private void ExitState(string stateName)
    {Debug.Log("Exit:" + stateName);
        if(stateName == _stateName)
            ExecuteExit();
    }

    private void ExecuteEnter()
    {
        _enterAction.Invoke();
    }

    private void ExecuteExit()
    {
        _exitAction.Invoke();
    }
}

