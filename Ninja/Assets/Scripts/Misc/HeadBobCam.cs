using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobCam : MonoBehaviour
{
    [SerializeField] private CharacterStateMachine playerStateMachine;
    [SerializeField] private List<string> activeInStates = new List<string>();

    [SerializeField] private AnimationCurve xMove;
    [SerializeField] private AnimationCurve yMove;

    [SerializeField] private new Rigidbody rigidbody;

    [SerializeField] private float speedTimerMultiplier = .2f;
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
        float velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).magnitude;
        
        if(activeInStates.Contains(state) == false || velocity == 0)
        {
            timer = 0;
            return;
        }

        timer += Time.deltaTime * velocity * speedTimerMultiplier;
        if(timer > 1f)
            timer -= 1f;
        
        Vector3 nextPosition = originPos + new Vector3(
            xMove.Evaluate(timer) * xValueMultiplier, 
            yMove.Evaluate(timer) * yValueMultiplier, 
            0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, nextPosition, .1f);
    }
}
