﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State _currentState;
    [SerializeField] private State[] _states;

    public string CurrentState { get{return _currentState.GetType().Name;}}

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
            _currentState.ExitState();

        _currentState = _states[stateIndex];
        _currentState.EnterState(this);
    }
}
