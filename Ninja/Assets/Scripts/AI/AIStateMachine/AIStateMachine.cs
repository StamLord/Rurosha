using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : StateMachine
{
    [SerializeField] private AwarenessAgent awarenessAgent;
    [SerializeField] private InputState inputState;
}
