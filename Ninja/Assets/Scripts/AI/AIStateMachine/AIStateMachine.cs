using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateMachine : StateMachine
{
    [SerializeField] private AwarenessAgent awarenessAgent;
    public AwarenessAgent AwarenessAgent {get{return awarenessAgent;}}
    [SerializeField] private AIInput aiInput;
    
    public StealthAgent enemy;
    public Vector3 enemyLastSeen;
    public Vector3 enemyLastDir;

    [SerializeField] private MeshRenderer meshRenderer;

    public bool CalculatePath(Vector3 target)
    {
        return aiInput.CalculatePath(target);
    }

    public void ClearPath()
    {
        aiInput.ClearPath();
    }

    public void SetDebugColor(Color color)
    {
        meshRenderer.material.color = color;
    }
}
