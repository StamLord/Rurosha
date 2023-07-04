using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaDirectional : WeaponObject, IHitboxResponder
{   
    [Header("Aim")]
    [SerializeField] private Direction9 direction;
    [SerializeField] float threshold = .5f;
    [SerializeField] Vector2 mouseInput;
    [SerializeField] float mouseAngle;

    [Header("Damage")]
    [SerializeField] private int softDamage = 20;
    [SerializeField] private int hardDamage = 10;
    [SerializeField] private float chanceToBleed = .25f;

    [Header("Experience")]
    [SerializeField] private float strengthExpGain = .01f;
    [SerializeField] private float dexterityExpGain = .01f;

    [Header("Hitbox")]
    [SerializeField] private Hitbox hitbox;

    [Header("Slicing")]
    [SerializeField] private float sliceForce = 2f;
    [SerializeField] private float maxSlicesPerCut = 10;
    [SerializeField] private List<GameObject> newSlices = new List<GameObject>();
    
    [Header("Animation")]
    [SerializeField] private float idleTransitionSpeed = .5f;
    private int attack_up, attack_upleft, attack_left, attack_downleft, attack_down, attack_downright, attack_right, attack_upright, attack_center;

    [Header("Combo")]
    [SerializeField] private int maxCombo = 3;
    [SerializeField]public List<Direction9> combo = new List<Direction9>();
    [SerializeField] private bool specialAttackReady;
    [SerializeField] private Direction9 lastAttackDirection;
    [SerializeField] private Direction9 lastComboAttackDirection;

    [Header("Special Attack")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float specialProjDistance;
    [SerializeField] private float specialProjSpeed;

    private void Start()
    {
        hitbox?.SetResponder(this);

        attack_up = Animator.StringToHash("attack_up");
        attack_upright = Animator.StringToHash("attack_upright");
        attack_right = Animator.StringToHash("attack_right");
        attack_downright = Animator.StringToHash("attack_downright");
        attack_down = Animator.StringToHash("attack_down");
        attack_downleft = Animator.StringToHash("attack_downleft");
        attack_left = Animator.StringToHash("attack_left");
        attack_upleft = Animator.StringToHash("attack_upleft");
        attack_center = Animator.StringToHash("attack_center");
    }

    public Direction9 GetDirection()
    {
        return direction;
    }

    private void Update()
    {
        ProcessMouseMovement();
        UpdateIdleAnimation();
        ProcessAttackInput();
    }

    private void ProcessMouseMovement()
    {
        if(inputState.MouseButton2.State != VButtonState.PRESSED) return;

        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if(mouseInput.magnitude < threshold) return;

        CalculateDirection();
    }

    private void CalculateDirection()
    {
        mouseAngle = Mathf.Atan2(mouseInput.x, mouseInput.y) * Mathf.Rad2Deg;
        if(mouseAngle > -22.5 && mouseAngle <= 22.5)
            SetDirection(Direction9.UP);
        else if(mouseAngle > 22.5 && mouseAngle <= 67.5)
            SetDirection(Direction9.UPRIGHT);
        else if(mouseAngle > 67.5 && mouseAngle <= 112.5)
            SetDirection(Direction9.RIGHT);
        else if(mouseAngle > 112.5 && mouseAngle <= 157.5)
            SetDirection(Direction9.DOWNRIGHT);
        else if(mouseAngle > 157.5 && mouseAngle <= 180 || mouseAngle < -157.5 && mouseAngle < -179)
            SetDirection(Direction9.DOWN);
        else if(mouseAngle <= -112.5 && mouseAngle > -157.5)
            SetDirection(Direction9.DOWNLEFT);
        else if(mouseAngle <= -67.5 && mouseAngle > -112.5)
            SetDirection(Direction9.LEFT);
        else if(mouseAngle <= -22.5 && mouseAngle > -67.5)
            SetDirection(Direction9.UPLEFT);
    }

    public void SetDirection(Direction9 newDirection)
    {
        direction = newDirection;
    }

    private void UpdateIdleAnimation()
    {   
        // Get target 2D coords of our blend tree
        float v = 0;
        float h = 0;

        switch(direction)
        {
            case Direction9.UP:
                h = 0;
                v = 1;
                break;
            case Direction9.UPRIGHT:
                h = 1;
                v = 1;
                break;
            case Direction9.RIGHT:
                h = 1;
                v = 0;
                break;
            case Direction9.DOWNRIGHT:
                h = 1;
                v = -1;
                break;
            case Direction9.DOWN:
                h = 0;
                v = -1;
                break;
            case Direction9.DOWNLEFT:
                h = -1;
                v = -1;
                break;
            case Direction9.LEFT:
                h = -1;
                v = 0;
                break;
            case Direction9.UPLEFT:
                h = -1;
                v = 1;
                break;
        }

        // Update paramteres of idle blend tree by lerping to target coords for smooth transition
        animator.SetFloat("V", Mathf.Lerp(animator.GetFloat("V"), v, idleTransitionSpeed * Time.deltaTime));
        animator.SetFloat("H", Mathf.Lerp(animator.GetFloat("H"), h, idleTransitionSpeed * Time.deltaTime));
    }

    private void ProcessAttackInput()
    {
        // Check if defending
        bool defending = inputState.Defend.State == VButtonState.PRESSED;
        animator.SetBool("DEFEND", defending);

        if(defending)
            return;

        // Check if attacking
        // Play attack animation based on current idle animation state
        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
        {
            // Make sure we are not mid attack
            int curState = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
           
            if( curState == attack_up ||
                curState == attack_upright ||
                curState == attack_right ||
                curState == attack_downright ||
                curState == attack_down ||
                curState == attack_downleft ||
                curState == attack_left ||
                curState == attack_upleft ||
                curState == attack_center )
                return;
            
            
            // Play animation based on direction
            switch(direction)
            {
                case Direction9.UP:
                    animator.Play("attack_up");
                    break;
                case Direction9.UPRIGHT:
                    animator.Play("attack_upright");
                    break;
                case Direction9.RIGHT:
                   animator.Play("attack_right");
                    break;
                case Direction9.DOWNRIGHT:
                    animator.Play("attack_downright");
                    break;
                case Direction9.DOWN:
                    animator.Play("attack_down");
                    break;
                case Direction9.DOWNLEFT:
                    animator.Play("attack_downleft");;
                    break;
                case Direction9.LEFT:
                    animator.Play("attack_left");
                    break;
                case Direction9.UPLEFT:
                   animator.Play("attack_upleft");
                    break;
            }

            lastAttackDirection = direction;
            if(specialAttackReady) SpecialAttack(direction);
                
        }
    }

    // Called by Hitbox on collision
    public void CollisionWith(Collider collider, Hitbox hitbox)
    {   
        Vector3 hitPoint = collider.ClosestPoint(transform.position);

        // Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        bool isHit = false;
        if(hurtbox)
            isHit = hurtbox.Hit(agent, softDamage, hardDamage, Vector3.up, hitbox.Velocity, DamageType.Slash);
        
        if(isHit)
        {   
            // Gain Exp
            charStats.IncreaseAttributeExp(AttributeType.STRENGTH, strengthExpGain);

            // Add to combo only if this hit is registered by local hitbox and not by projectile aka Special Attack
            if(hitbox == this.hitbox)
                AddCombo(lastAttackDirection);
        }

        //Slice
        Sliceable sliceable = collider.GetComponent<Sliceable>();
        if(sliceable)
        {
            // Check if max sliced objects were created
            if(newSlices.Count >= maxSlicesPerCut)
                return;

            // Check if this is a sliced object that was created by this same attack
            foreach(GameObject s in newSlices)
                if(s == collider.gameObject)
                    return;

            GameObject toSlice = collider.gameObject;

            // Use local reference or passed parameter for hitbox (In case of a special attack the projectile has his own hitbox)
            Vector3 normal =  hitbox.transform.right;
            Vector3 transformedNormal = ((Vector3)(toSlice.transform.localToWorldMatrix.transpose * normal)).normalized;
            Plane plane = new Plane(transformedNormal, toSlice.transform.InverseTransformPoint(hitbox.transform.position));

            StartCoroutine(SliceCoroutine(plane, toSlice));
        }
    }

    public void GuardedBy(Collider collider, Hitbox hitbox)
    {
        // Play guarded animation
        // Depelte stamina
        // Stun if run out of stamina
    }

    public void PerfectGuardedBy(Collider collider, Hitbox hitbox)
    {

    }

    private IEnumerator SliceCoroutine(Plane plane, GameObject toSlice)
    {
        GameObject[] slices = Slicer.Slice(plane, toSlice);

        // Add new sliced objects to a list of objects ignored in any further collisions
        foreach(GameObject s in slices)
            newSlices.Add(s);

        Destroy(toSlice);
        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        rigidbody.AddForce(hitbox.transform.up * sliceForce, ForceMode.Impulse);

        yield return null;
    }

    public void UpdateColliderState(bool newState)
    {
        if(newState == false)
            newSlices.Clear();
    }

    private void AddCombo(Direction9 hitDirection)
    {
        // Make sure it's not the last direction added to combo
        // Handles cases when combo is cleared but another collision registers by same attack immedietly adding same direction
        if(lastComboAttackDirection == hitDirection)
            return;

        // Make sure direction is unique in combo
        if(combo.Contains(hitDirection))
            return;
        
        combo.Add(hitDirection);
        lastComboAttackDirection = hitDirection;

        if(combo.Count >= maxCombo)
        {
            combo.Clear();
            specialAttackReady = true;
        }
    }

    private void SpecialAttack(Direction9 direction)
    {
        specialAttackReady = false;
        float z = 0;

        if(direction == Direction9.LEFT || direction == Direction9.RIGHT)
            z = 90;
        else if(direction == Direction9.UPLEFT || direction == Direction9.DOWNRIGHT)
            z = 45;
        else if(direction == Direction9.UPRIGHT || direction == Direction9.DOWNLEFT)
            z = -45;
        
        GameObject p = Instantiate(projectile, hitbox.transform.position + transform.forward, transform.rotation);
        p.transform.localRotation = Quaternion.Euler(p.transform.localEulerAngles.x, p.transform.localEulerAngles.y ,z);
        p.GetComponent<Hitbox>().SetResponder(this);
        StartCoroutine("SpecialProjectile", p);
    }

    IEnumerator SpecialProjectile(GameObject projectile)
    {
        Vector3 origin = projectile.transform.position;
        while(Vector3.Distance(projectile.transform.position, origin) < specialProjDistance)
        {
            projectile.transform.position += projectile.transform.forward * specialProjSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Destroy(projectile);
    }

}
