using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Usable
{
    [SerializeField] private bool _state;
    public bool state { get { return _state; } }

    [SerializeField] private string onText;
    [SerializeField] private string offText;
    
    public delegate void StateChangeDelegate(bool state);
    public event StateChangeDelegate StateChangeEvent;

    public override void Use(Interactor interactor)
    {
        base.Use(interactor);
        _state = !_state;

        interactionText = (_state)? onText : offText;

        if(StateChangeEvent != null)
            StateChangeEvent(_state);
    }
}
