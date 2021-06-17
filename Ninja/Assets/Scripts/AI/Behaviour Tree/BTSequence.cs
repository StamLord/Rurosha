using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTNode
{
    private List<BTNode> _nodes = new List<BTNode>();

    public BTSequence (BTBrain brain, List<BTNode> nodes)
    {
        _brain = brain;
        _nodes = nodes;
    }

    public override NodeStates Evaluate()
    {
        bool anyChildRunning = false;

        foreach(BTNode n in _nodes)
        {
            switch(n.Evaluate())
            {
                case NodeStates.FAILURE:
                    _nodeState = NodeStates.FAILURE;
                    return _nodeState;
                case NodeStates.SUCCESS:
                    continue;
                case NodeStates.RUNNING:
                    anyChildRunning = true;
                    continue;
                default:
                    _nodeState = NodeStates.SUCCESS;
                    return _nodeState;
            }
        }

        _nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
        return _nodeState;
    }
}
