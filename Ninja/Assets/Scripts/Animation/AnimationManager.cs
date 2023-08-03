using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStateMachine stateMachine;
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Animator animator;
    [SerializeField] private InputState inputState;
    
    [Header("Hit Animations")]
    [SerializeField] private int hitAnimations = 1;

    [Header("Multiple by speed of NavMeshAgent/Rigidbody")]
    [SerializeField] private bool useNavMeshAgent;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private new Rigidbody rigidbody;
    
    [Header("Running Speed")]
    [SerializeField] private float runSpeed = 10;

    [Header("IK")]
    [SerializeField] private Transform ikLookTarget;
    [SerializeField] private Transform ikLeftTarget;
    [SerializeField] private Transform ikRightTarget;

    private void OnValidate() 
    {
        animator = GetComponent<Animator>();    
    }

    private void Start()
    {
        if(stateMachine) stateMachine.OnStateEnter += StateUpdate;
        if(characterStats) characterStats.OnHit += AnimateHit;
        if(weaponManager) weaponManager.ChangeWeaponEvent += WeaponUpdate;
    }
    
    private void LateUpdate()
    {
        if(inputState == null || rigidbody == null) return;

        Vector3 velocity = inputState.AxisInput;
        velocity *= (useNavMeshAgent)? navMeshAgent.velocity.magnitude : rigidbody.velocity.magnitude;

        animator.SetFloat("x", velocity.x);
        animator.SetFloat("z", velocity.z / runSpeed);
        animator.SetFloat("y", rigidbody.velocity.y);
        animator.SetBool("crouch", inputState.Crouch.State == VButtonState.PRESSED);

        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
            animator.SetTrigger("LMB");

        if(inputState.MouseButton2.State == VButtonState.PRESS_START)
            animator.SetTrigger("RMB");
    }

    private void StateUpdate(string stateName)
    {
        switch(stateName)
        {
            case "GroundedState":
                animator.CrossFade("grounded", .1f);
                break;
            case "AirState":
                animator.CrossFade("air", .1f);
                break;
            case "SimpleJumpState":
                animator.CrossFade("air", .1f);
                break;
            case "CrouchState":
                animator.CrossFade("crouch", .1f);
                break;
            case "DashState":
                animator.CrossFade("dash", .01f);
                break;
        }
    }

    private void WeaponUpdate(WeaponType weaponType)
    {
        animator.SetInteger("Weapon", (int)weaponType);
        animator.SetTrigger("ChangeWeapon");
    }

    private void AnimateHit(int softDamage, int hardDamage)
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if(state.IsName("Idle") || state.IsName("Attack1"))
        {
            animator.SetInteger("Hit Index", Random.Range(0, hitAnimations));
            animator.SetTrigger("Hit");
        }
    }

    private void OnAnimatorIK(int layerIndex) 
    {
        if(ikRightTarget)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
            animator.SetIKPosition(AvatarIKGoal.RightHand, ikRightTarget.position);
        }

        if(ikLeftTarget)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);  
            animator.SetIKPosition(AvatarIKGoal.LeftHand, ikLeftTarget.position);
        }

        if(ikLookTarget)
        {
            animator.SetLookAtWeight(1, 0, 1, 0);
            animator.SetLookAtPosition(ikLookTarget.position);
        }
    }
}
