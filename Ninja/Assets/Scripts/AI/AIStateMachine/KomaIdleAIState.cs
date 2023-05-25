using System.Collections;
using UnityEngine;

public class KomaIdleAIState : AIState
{
    [SerializeField] private Animator animator;
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
        if(subState == KomaState.ASLEEP)
        {
            animator.Play("idle_asleep");
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
        StartCoroutine("Awakening");
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

        animator.CrossFade("idle_awake", .5f);
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
        animator.CrossFade("idle_asleep", .5f);
    }

    private void SetActiveObjects(bool state)
    {
        foreach(GameObject comp in disabledGameObjects)
            comp.SetActive(state);
    }

    private void OnDrawGizmos() 
    {
        if(debug == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, karmaDetectRadius);
    }
}
