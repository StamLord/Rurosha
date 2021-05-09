using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State _currentState;
    [SerializeField] private State[] _states;

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
            Debug.LogWarning("Out of bounds state index!");
            return;
        }

        if(_currentState)
            _currentState.ExitState();

        _currentState = _states[stateIndex];
        _currentState.EnterState(this);
    }
}
