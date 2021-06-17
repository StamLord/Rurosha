using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTActionNode : BTNode
{
    public delegate NodeStates ActionNodeDelegate();
    private ActionNodeDelegate _action;

    public BTActionNode(BTBrain brain, ActionNodeDelegate action)
    {
        _brain = brain;
        _action = action;
    }

    public override NodeStates Evaluate()
    {
        switch(_action())
        {
            case NodeStates.SUCCESS:
                _nodeState = NodeStates.SUCCESS;
                return _nodeState;
            case NodeStates.FAILURE:
                _nodeState = NodeStates.FAILURE;
                return _nodeState;
            case NodeStates.RUNNING:
                _nodeState = NodeStates.RUNNING;
                return _nodeState;
            default:
                _nodeState = NodeStates.FAILURE;
                return _nodeState;

        }
    }

}
