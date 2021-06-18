using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobCam : MonoBehaviour
{
    [SerializeField] private PlayerControls playerStateMachine;
    [SerializeField] private List<string> activeInStates = new List<string>();

    [SerializeField] private AnimationCurve xMove;
    [SerializeField] private AnimationCurve yMove;

    [SerializeField] private new Rigidbody rigidbody;

    [SerializeField] private float speedTimerMultiplier = 1f;
    [SerializeField] private float xValueMultiplier = 1f;
    [SerializeField] private float yValueMultiplier = 1f;
    
    private Vector3 originPos;
    private float timer;

    void Start()
    {
        originPos = transform.localPosition;
    }

    void Update()
    {
        string state = playerStateMachine.CurrentState;
        
        if(activeInStates.Contains(state) == false)
            return;

        float velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).magnitude;

        timer += Time.deltaTime * velocity * speedTimerMultiplier;
        if(timer > 1f || velocity == 0)
            timer = 0;

        Vector3 newPosition = Vector3.Lerp(originPos, originPos + new Vector3(xMove.Evaluate(timer) * velocity * xValueMultiplier, yMove.Evaluate(timer) * velocity * yValueMultiplier, 0), velocity);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, .1f);
    }
}
