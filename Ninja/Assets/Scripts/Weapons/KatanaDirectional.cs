using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaDirectional : WeaponObject, IHitboxResponder
{   
    [Header("Aim")]
    [SerializeField] private Direction9 direction;
    [SerializeField] float threshold = .5f;
    [SerializeField] Vector2 mouseInput;
    [SerializeField] float mouseDelta;
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
    
    private void Start()
    {
        hitbox?.SetResponder(this);
    }

    public Direction9 GetDirection()
    {
        return direction;
    }

    private void Update()
    {
        ProcessMouseMovement();
        ProcessAttackInput();
    }

    private void ProcessMouseMovement()
    {
        if(inputState.MouseButton2.State != VButtonState.PRESSED) return;

        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseDelta = mouseInput.magnitude;
        if(mouseDelta < threshold) return;

        CalculateDirection();
    }

    private void CalculateDirection()
    {
        mouseAngle = Mathf.Atan2(mouseInput.x, mouseInput.y) * Mathf.Rad2Deg;
        if(mouseAngle > -22.5 && mouseAngle <= 22.5)
            direction = Direction9.UP;
        else if(mouseAngle > 22.5 && mouseAngle <= 67.5)
            direction = Direction9.UPRIGHT;
        else if(mouseAngle > 67.5 && mouseAngle <= 112.5)
            direction = Direction9.RIGHT;
        else if(mouseAngle > 112.5 && mouseAngle <= 157.5)
            direction = Direction9.DOWNRIGHT;
        else if(mouseAngle > 157.5 && mouseAngle <= 180 || mouseAngle < -157.5 && mouseAngle < -179)
            direction = Direction9.DOWN;
        else if(mouseAngle <= -112.5 && mouseAngle > -157.5)
            direction = Direction9.DOWNLEFT;
        else if(mouseAngle <= -67.5 && mouseAngle > -112.5)
            direction = Direction9.LEFT;
        else if(mouseAngle <= -22.5 && mouseAngle > -67.5)
            direction = Direction9.UPLEFT;
    }

    private void ProcessAttackInput()
    {
        if(inputState.MouseButton1.State == VButtonState.PRESS_START)
        {
            switch(direction)
            {
                case Direction9.UP:
                    animator.SetTrigger("UP");
                    break;
                case Direction9.UPRIGHT:
                    animator.SetTrigger("UPRIGHT");
                    break;
                case Direction9.RIGHT:
                    animator.SetTrigger("RIGHT");
                    break;
                case Direction9.DOWNRIGHT:
                    animator.SetTrigger("DOWNRIGHT");
                    break;
                case Direction9.DOWN:
                    animator.SetTrigger("DOWN");
                    break;
                case Direction9.DOWNLEFT:
                    animator.SetTrigger("DOWNLEFT");
                    break;
                case Direction9.LEFT:
                    animator.SetTrigger("LEFT");
                    break;
                case Direction9.UPLEFT:
                    animator.SetTrigger("UPLEFT");
                    break;
            }
        }
    }

    // Called by Hitbox on collision
    public void CollisionWith(Collider collider)
    {   
        Vector3 hitPoint = collider.ClosestPoint(transform.position);

        // Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        bool isHit = false;
        if(hurtbox)
            isHit = hurtbox.Hit(softDamage, hardDamage, DamageType.Slash, direction);
        
        if(isHit)
        {   
            // Gain Exp
            charStats.IncreaseAttributeExp("strength", strengthExpGain);
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

            Vector3 normal = hitbox.transform.right;
            Vector3 transformedNormal = ((Vector3)(toSlice.transform.localToWorldMatrix.transpose * normal)).normalized;
            Plane plane = new Plane(transformedNormal, toSlice.transform.InverseTransformPoint(hitbox.transform.position));

            StartCoroutine(SliceCoroutine(plane, toSlice));
        }
    }

    private IEnumerator SliceCoroutine(Plane plane, GameObject toSlice)
    {
        GameObject[] slices = Assets.Scripts.Slicer.Slice(plane, toSlice);

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

}
