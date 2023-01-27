using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateMachine : StateMachine
{
    [SerializeField] private CharacterStats characterStats;
    public CharacterStats CharacterStats {get{return characterStats;}}
    
    [SerializeField] private AwarenessAgent awarenessAgent;
    public AwarenessAgent AwarenessAgent {get{return awarenessAgent;}}

    [SerializeField] private StealthAgent stealthAgent;
    public StealthAgent StealthAgent {get{return stealthAgent;}}
    
    [SerializeField] private AIInput aiInput;
    
    public StealthAgent enemy;
    public Vector3 enemyLastSeen;
    public Vector3 enemyLastDir;

    private static bool isDebug;
    
    public enum StateName
    {
        IDLE,
        SEARCH,
        FIGHT,
        FLEE
    };

    [Space(20)]

    [Header("States")]

    [SerializeField] private AIState idle;
    [SerializeField] private AIState search;
    [SerializeField] private AIState fight;
    [SerializeField] private AIState flee;

    private void Start()
    {
        DebugCommandDatabase.AddCommand(new DebugCommand(
                "debugaistate", 
                "Sets debug of AIStateMachine to true or false", 
                "debugaistate <1/0>", 
                (string[] parameters) => {
                    switch(parameters[0])
                    {
                        case "0":
                            isDebug = false;
                            return "AIStateMachine debug set to False";
                        case "1":
                            isDebug = true;
                            return "AIStateMachine debug set to True";
                    }
                    return "Parameter should be 1 or 0";
                }));
        SwitchState(0);
    }

    public void SwitchState(StateName state)
    {
        switch(state)
        {
            case StateName.IDLE:
                SwitchState(idle);
                break;
            case StateName.SEARCH:
                SwitchState(search);
                break;
            case StateName.FIGHT:
                SwitchState(fight);
                break;
            case StateName.FLEE:
                SwitchState(flee);
                break;
        }
    }

    public bool CalculatePath(Vector3 target)
    {
        return aiInput.CalculatePath(target);
    }

    public void ClearPath()
    {
        aiInput.ClearPath();
    }

    public Vector3 GetNextPosition()
    {
        return aiInput.GetPathPosition();
    }

    public Vector3 GetLastPosition()
    {
        return aiInput.GetLastPosition();
    }

    public void PressButton(string button)
    {
        aiInput.PressButton(button);
    }

    public void HoldButton(string button)
    {
        aiInput.HoldButton(button);
    }

    public void StopHoldButton(string button)
    {
        aiInput.StopHoldButton(button);
    }

    public void StartOverrideMovement(Vector3 input)
    {
        aiInput.StartOverrideMovement(input);
    }

    public void StopOverrideMovement()
    {
        aiInput.StopOverrideMovement();
    }

    private void OnGUI() 
    {
        if(isDebug == false) return;
        
        Camera cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        if(viewportPos.x <= 0 || viewportPos.x >= 1 || viewportPos.y <= 0 || viewportPos.y >= 1 || viewportPos.z < 0) return;

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(screenPos.x, screenPos.y - 50, 100, 50), CurrentState);
    }
}
