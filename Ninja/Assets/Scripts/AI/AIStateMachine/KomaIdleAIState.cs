using System.Collections;
using UnityEngine;

public class KomaIdleAIState : AIState
{
    [SerializeField] private Transform statuePoint;
    [SerializeField] private MeshGlow meshGlow;

    [SerializeField] private float karmaDetectRadius = 10;
    [SerializeField] private float negativeKarmaThreshold = -5;

    [SerializeField] private float minimumAwakeTime = 30f;

    [SerializeField] private float toAwakeTime = 3f;
    [SerializeField] private float toSleepTime = 3f;

    [SerializeField] private GameObject[] disabledGameObjects;

    private enum KomaState {ASLEEP, AWAKENING, AWAKE, FALLING_ASLEEP};
    [SerializeField] private KomaState subState;

    private float subStateStartTime;
    private float subStateTime { get { return Time.time - subStateStartTime;}}
    
    protected override void OnEnterState()
    {
        AIStateMachine.CharacterStats.OnHitBy += Hit;
        AIStateMachine.AwarenessAgent.OnSeeAgent += SeeTarget;
        AIStateMachine.SquadAgent.OnGetMessage += GetMessage;

        if(subState == KomaState.ASLEEP)
        {
            Animator?.Play("idle_asleep");
            SetActiveObjects(false);
        }
    }

    public override void OnStateUpdate()
    {
        switch(subState)
        {
            case KomaState.ASLEEP:
                if(IsBadKarmaInRange(karmaDetectRadius))
                    Awaken();
                break;
            case KomaState.AWAKE:
                // We stay awake for some time before going back to sleep
                if(subStateTime < minimumAwakeTime)
                    break;
                
                // Go back to sleep at statue position
                float distance = Vector3.Distance(transform.position, statuePoint.position);
                if(distance <= .1f)
                    FallAsleep();
                else
                    MoveTo(statuePoint.position);
                break;
        }
    }

    protected override void OnExitState()
    {
        AIStateMachine.CharacterStats.OnHitBy -= Hit;
        AIStateMachine.AwarenessAgent.OnSeeAgent -= SeeTarget;
    }

    private void GetMessage(string message, SquadAgent sender)
    {
        switch(message)
        {
            case "Awaken": // This message is sent by squad memeber that also awakens
                Awaken();
                break;
            case "Fight": // This message is sent by squad memeber that enters fight state
                SwitchState(AIStateMachine.StateName.FIGHT);
                break;
        }
    }

    private bool IsBadKarmaInRange(float radius)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider col in cols)
        {
            // Colliders are children of the main object so we check root
            CharacterStats stats = col.transform.root.GetComponent<CharacterStats>();
            if(stats && stats.Karma < negativeKarmaThreshold)
                return true;
        }
        return false;
    }

    private void Awaken()
    {
        if(subState == KomaState.AWAKE || subState == KomaState.AWAKENING) return;
        
        StartCoroutine("Awakening");
        SquadAgent.SendMessage("Awaken");
    }

    private IEnumerator Awakening()
    {
        SwitchKomaState(KomaState.AWAKENING);

        meshGlow?.Glow(5f);

        float startTime = Time.time;
        while (Time.time - startTime <= toAwakeTime)
        {
            float p = (Time.time - startTime) / toAwakeTime;
            yield return null;
        }

        SetActiveObjects(true);
        
        MoveTo(transform.position + transform.forward * 2f);
        SwitchKomaState(KomaState.AWAKE);

        Animator?.CrossFade("idle_awake", .5f);
    }

    private void SwitchKomaState(KomaState state)
    {
        subState = state;
        subStateStartTime = Time.time;
    }

    private void FallAsleep()
    {
        StartCoroutine("FallingAsleep");
    }

    private IEnumerator FallingAsleep()
    {
        SwitchKomaState(KomaState.FALLING_ASLEEP);

        float startTime = Time.time;
        while (Time.time - startTime <= toSleepTime)
        {
            //float p = (Time.time - startTime) / toSleepTime;
            LookTowards(statuePoint.forward);
            yield return null;
        }

        SetActiveObjects(false);

        SwitchKomaState(KomaState.ASLEEP);
        Animator?.CrossFade("idle_asleep", .5f);
    }

    private void SetActiveObjects(bool state)
    {
        foreach(GameObject comp in disabledGameObjects)
            comp.SetActive(state);
    }

    private void Hit(StealthAgent agent)
    {
        // Only care if we are awake
        if(subState != KomaState.AWAKE) return;

        // If an ally we don't care
        if(IsAlly(agent))
            return;
        
        AIStateMachine.enemy = agent;

        // If enemy is visible we switch to Fight
        foreach(StealthAgent a in AIStateMachine.AwarenessAgent.VisibleAgents)
        {
            if(agent == a)
            {
                SwitchState(AIStateMachine.StateName.FIGHT);
                return;
            }
        }

        // Otherwise, switch to SearchAIState
        SwitchState(AIStateMachine.StateName.SEARCH);
    }

    private void SeeTarget(StealthAgent agent)
    {
        // Only care if we are awake
        if(subState != KomaState.AWAKE) return;

        // If not an enemy, do nothing
        if(IsEnemy(agent) == false)
            return;

        AIStateMachine.enemy = agent;
        SwitchState(AIStateMachine.StateName.FIGHT);
        SquadAgent.SendMessage("Fight");
    }

    public new bool IsEnemy(StealthAgent agent)
    {
        // Check Karama
        return true;
    }

    private void OnDrawGizmos() 
    {
        if(debug == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, karmaDetectRadius);
    }
}
