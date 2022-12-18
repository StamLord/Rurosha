using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

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
    {
        if(stateName == _stateName)
            ExecuteEnter();
    }

    private void ExitState(string stateName)
    {
        if(stateName == _stateName)
            ExecuteExit();
    }

    private void ExecuteEnter()
    {
        try
        {
            _enterAction.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log("Exception while running ExecuteEnter from StateListener: " + e);
        }
    }

    private void ExecuteExit()
    {
        try
        {
            _exitAction.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log("Exception while running ExecuteExit from StateListener: " + e);
        }
    }
}

