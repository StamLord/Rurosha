using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [Header("State Machine")]
    [SerializeField] private State currentState;
    [SerializeField] private State[] states;
    [SerializeField] protected State defaultState;
    
    [SerializeField] private bool debugLogs;
    public string CurrentState { get{return currentState.GetType().Name;}}

    public delegate void stateEnter(string stateName);
    public event stateEnter OnStateEnter;

    public delegate void stateExit(string stateName);
    public event stateExit OnStateExit;

    private void Start()
    {
        SwitchState(defaultState);
    }

    private void FixedUpdate()
    {
        currentState?.OnStateUpdate();
    }

    public void SwitchState(State state)
    {
        // Exit State
        if(currentState)
        {
            if(debugLogs)
                Debug.Log("Exiting state: " + currentState);

            currentState.ExitState();
            
            // Event
            if(OnStateExit != null)
                OnStateExit(CurrentState);
        }

        // Enter State
        if(debugLogs)
            Debug.Log("Entering state: " + state);
        
        currentState = state;
        currentState.EnterState(this);

        // Event
        if(OnStateEnter != null)
            OnStateEnter(CurrentState);


    }

    [System.Obsolete]
    public void SwitchState(int stateIndex)
    {
        if(stateIndex >= states.Length)
        {
            Debug.LogWarning("Out of bounds state index!" + stateIndex + "/" + states.Length + "GameObject: " + transform.gameObject.name);
            return;
        }

        if(currentState)
        {
            // Event for exiting state
            if(OnStateExit != null)
                OnStateExit(CurrentState);
            
            currentState.ExitState();
        }

        currentState = states[stateIndex];
        currentState.EnterState(this);

        if(debugLogs)
            Debug.Log("Entering state: " + stateIndex);

        // Event for new state
        if(OnStateEnter != null)
            OnStateEnter(CurrentState);
    }
}
