using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [Header("State Machine")]
    [SerializeField] private State _currentState;
    [SerializeField] private State[] _states;
    
    [SerializeField] private bool _debugLogs;
    public string CurrentState { get{return _currentState.GetType().Name;}}

    public delegate void stateEnter(string stateName);
    public event stateEnter OnStateEnter;

    public delegate void stateExit(string stateName);
    public event stateExit OnStateExit;

    void Start()
    {
        SwitchState(0);
    }

    void FixedUpdate()
    {
        _currentState.OnStateUpdate();
    }

    public void SwitchState(int stateIndex)
    {
        if(stateIndex >= _states.Length)
        {
            Debug.LogWarning("Out of bounds state index!" + stateIndex + "/" + _states.Length + "GameObject: " + transform.gameObject.name);
            return;
        }

        if(_currentState)
        {
            // Event for exiting state
            if(OnStateExit != null)
                OnStateExit(CurrentState);
            
            _currentState.ExitState();
        }

        _currentState = _states[stateIndex];
        _currentState.EnterState(this);

        if(_debugLogs)
            Debug.Log("Entering state: " + stateIndex);

        // Event for new state
        if(OnStateEnter != null)
            OnStateEnter(CurrentState);
    }
}
