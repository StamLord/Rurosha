using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeStates {FAILURE, SUCCESS, RUNNING}

public abstract class BTNode
{
    public delegate NodeStates NodeReturn();

    protected BTBrain _brain;
    protected NodeStates _nodeState;

    public NodeStates NodeState {get {return _nodeState;}}

    public BTNode() 
    {

    }

    public abstract NodeStates Evaluate();
    
}
