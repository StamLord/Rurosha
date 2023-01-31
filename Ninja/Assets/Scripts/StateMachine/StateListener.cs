using UnityEngine;
using UnityEngine.Events;
using System;

public class StateListener : MonoBehaviour
{
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private string[] stateNames;
    [SerializeField] private UnityEvent enterAction;
    [SerializeField] private UnityEvent exitAction;

    private void Start() 
    {
        if(stateMachine)
        {
            stateMachine.OnStateEnter += EnterState;
            stateMachine.OnStateExit += ExitState;
        }
    }

    private void EnterState(string stateName)
    {
        foreach(string name in stateNames)
        {
            if(name == stateName)
            {
                ExecuteEnter();
                break;
            }
        }
    }

    private void ExitState(string stateName)
    {
        foreach(string name in stateNames)
        {
            if(name == stateName)
            {
                ExecuteExit();
                break;
            }
        }
    }

    private void ExecuteEnter()
    {
        try
        {
            enterAction.Invoke();
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
            exitAction.Invoke();
        }
        catch (Exception e)
        {
            Debug.Log("Exception while running ExecuteExit from StateListener: " + e);
        }
    }
}

