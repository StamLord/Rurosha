using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Usable
{
    [SerializeField] private bool _state;
    public bool state { get { return _state; } }
    
    public delegate void StateChangeDelegate(bool state);
    public event StateChangeDelegate StateChangeEvent;

    public override void Use(Interactor interactor)
    {
        base.Use(interactor);
        _state = !_state;

        if(StateChangeEvent != null)
            StateChangeEvent(_state);
    }
}
