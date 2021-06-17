using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTNode
{
    protected List<BTNode> _nodes = new List<BTNode>();

    public BTSelector (BTBrain brain, List<BTNode> nodes)
    {
        _brain = brain;
        _nodes = nodes;
    }

    public override NodeStates Evaluate()
    {
        foreach(BTNode n in _nodes)
        {
            switch(n.Evaluate())
            {
                case NodeStates.FAILURE:
                    continue;
                case NodeStates.SUCCESS:
                    _nodeState = NodeStates.SUCCESS;
                    return _nodeState;
                case NodeStates.RUNNING:
                    _nodeState = NodeStates.RUNNING;
                    return _nodeState;
                default:
                    continue;
            }
        }

        _nodeState = NodeStates.FAILURE;
        return _nodeState;
    }
}
