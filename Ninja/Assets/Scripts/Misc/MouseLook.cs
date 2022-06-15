using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Camera Setting")]
    [SerializeField] private Vector2 sensitivity = new Vector2(100f,80f);
    [SerializeField] private Transform playerBody;
    [SerializeField] private AirState airState;
    [SerializeField] private CrouchState crouchState;
    [SerializeField] private SitState sitState;
    [SerializeField] private WallRunState wallRunState;
    
    //[SerializeField] private LookState _lookState = LookState.TURN_BODY;

    float xRotation = 0f;

    [SerializeField] private InputState inputState;
    [SerializeField] private float maxRotZ = 1f;
    [SerializeField] private float rotSpeedZ = 2f;

    [Header("Roll FX")]
    [SerializeField] private float rollDuration = .5f;
    [SerializeField] private bool rolling;
    [SerializeField] private AnimationCurve rollSpeedCurve;

    [Header("Sit FX")]
    [SerializeField] private float sitHeight;
    [SerializeField] float standToSitTransitionDuration = .7f;
    [SerializeField] float sitToStandTransitionDuration = .1f;

    [Header("Crouch FX")]
    [SerializeField] private float standHeight;
    [SerializeField] private float crouchHeight;
    [SerializeField] float crouchHeightTransitionDuration = .1f;

    private Coroutine heightAnimCoroutine;
    private bool isAnimatingHeight;

    [Header("Climb Ledge FX")]
    [SerializeField] private float maxTilt = 20f;
    [SerializeField] private float ledgeTiltDuration = .5f;
    [SerializeField] private bool isClimbingLedge;

    [Header("Wall Run FX")]
    [SerializeField] private float wallRunTilt = 20f;
    [SerializeField] private float wallRunDuration = .5f;
    [SerializeField] private bool isWallRunning;

    [SerializeField] private bool disabled;

    void Start()
    {
        UIManager.OnDisableMouse += Disable;
        UIManager.OnEnableMouse += Enable;

        airState.OnVaultStart += StartClimbLedgeTilt;
        airState.OnRollStart += StartRoll;
        crouchState.OnCrouchStart += StartCrouch;
        crouchState.OnCrouchEnd += EndCrouch;
        wallRunState.OnRunStart += StartWallRun;
        wallRunState.OnRunEnd += EndWallRun;
        sitState.OnSitStart += StartSit;
        sitState.OnSitEnd += EndSit;
    }
    
    void LateUpdate()
    {
        if(disabled) return;

        // Get Input Rotations
        float mouseX = Input.GetAxis("Mouse X") * sensitivity.x;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity.y;

        // Invert
        xRotation -= mouseY;

        // Clamp
        xRotation = Mathf.Clamp(this.xRotation, -90f, 90f);

        // Movement Feedback
        Vector3 inputVector = inputState.AxisInput;

        // Handle Z rotation
        float zRotation = transform.localEulerAngles.z;

        if(isWallRunning == false) // if WallRunning, coroutine will handle z rotation
        {
            float targetZRot = (inputVector.x > 0)? -maxRotZ : (inputVector.x < 0)? maxRotZ : 0f;
            float currentZ = transform.localRotation.eulerAngles.z;
            currentZ = (currentZ > 180)? currentZ - 360 : currentZ;
            float deltaZ = (targetZRot - currentZ) * rotSpeedZ * Time.deltaTime;
            zRotation =  currentZ + deltaZ;

            if(currentZ > targetZRot)
                zRotation = Mathf.Max(zRotation, targetZRot);
            else if(currentZ < targetZRot)
                zRotation = Mathf.Min(zRotation, targetZRot);
        }

        if(rolling == false)
        {
            // Rotate Camera
            transform.localRotation = Quaternion.Euler(xRotation, 0f, zRotation);
            // Rotate transform
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    private void Enable()
    {
        disabled = false;
    }

    private void Disable()
    {
        disabled = true;
    }

    #region Sit

    private void StartSit()
    {
        if(isAnimatingHeight) StopCoroutine(heightAnimCoroutine);
        heightAnimCoroutine = StartCoroutine(HeightTransition(sitHeight, standToSitTransitionDuration));
    }

    private void EndSit()
    {
        if(isAnimatingHeight) StopCoroutine(heightAnimCoroutine);
        heightAnimCoroutine = StartCoroutine(HeightTransition(standHeight, sitToStandTransitionDuration));
    }
    
    #endregion

    #region Crouch

    private void StartCrouch()
    {
        if(isAnimatingHeight) StopCoroutine(heightAnimCoroutine);
        heightAnimCoroutine = StartCoroutine(HeightTransition(crouchHeight, crouchHeightTransitionDuration));
    }

    private void EndCrouch()
    {
        if(isAnimatingHeight) StopCoroutine(heightAnimCoroutine);
        heightAnimCoroutine = StartCoroutine(HeightTransition(standHeight, crouchHeightTransitionDuration));
    }

    private IEnumerator HeightTransition(float newHeight, float duration)
    {
        isAnimatingHeight = true;

        // Wait 1 frame so we can check if airState is active 
        // since This function is called on crouch OnStateExit and before air OnStateEnter
        yield return null;

        // Don't animate camera while we are falling.
        // This prevents weird camera movement up when we end crouch due to falling
        while(airState.IsActive)
            yield return null;
        
        float startTime = Time.time;
        float startHeight = transform.localPosition.y;
        while(transform.localPosition.y != newHeight)
        {
            float p = (Time.time - startTime) / duration;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(startHeight, newHeight, p), transform.localPosition.z);
            yield return null;
        }
        isAnimatingHeight = false;
    }

    #endregion

    #region Wall Run

    private void StartWallRun(bool left)
    {
        StartCoroutine("StartWalRunTilt", left);
    }

    private void EndWallRun()
    {
        StartCoroutine("EndWalRunTilt");
    }

    private IEnumerator StartWalRunTilt(bool left)
    {
        isWallRunning = true;
        float startTime = Time.time;
        float startZ = transform.localRotation.eulerAngles.z;
        if(startZ > 180f) startZ -= 360f;
        float angle = (left)? -wallRunTilt : wallRunTilt;
        if(angle > 180f) angle -= 360f;

        while (Time.time - startTime < wallRunDuration)
        {
            float p = (Time.time - startTime) / wallRunDuration;

            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, Mathf.Lerp(startZ, angle, p));
            yield return null;
        }
    }

    private IEnumerator EndWalRunTilt()
    {
        float startTime = Time.time;
        float startZ = transform.localRotation.eulerAngles.z;
        if(startZ > 180f) startZ -= 360f;

        while (Time.time - startTime < wallRunDuration)
        {
            float p = (Time.time - startTime) / wallRunDuration;
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, Mathf.Lerp(startZ, 0, p));
            yield return null;
        }

        isWallRunning = false;
    }

    #endregion

    #region Climb Ledge

    private void StartClimbLedgeTilt()
    {
        if(isClimbingLedge)
            return;

        StartCoroutine("ClimbLedgeTilt");
    }

    private IEnumerator ClimbLedgeTilt()
    {
        isClimbingLedge = true;
        float startTime = Time.time;

        while (Time.time - startTime < ledgeTiltDuration)
        {
            float p = (Time.time - startTime ) / ledgeTiltDuration;
            float z = 0;

            if (p < .5f)
                z = Mathf.Lerp(0, maxTilt, p);
            else
                z = Mathf.Lerp(maxTilt, 0, p);

            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
            yield return null;
        }

        isClimbingLedge = false;
    }

    #endregion

    #region Roll

    private void StartRoll()
    {
        StartCoroutine("Roll");
    }

    private IEnumerator Roll()
    {
        rolling = true;
        Vector3 startRot = transform.localEulerAngles;
        Vector3 endRot = transform.localEulerAngles + new Vector3(360, 0 ,0);
        float start = Time.time;
        while (Time.time - start < rollDuration)
        {
            float p = (Time.time - start) / rollDuration;
            transform.localEulerAngles = Vector3.Lerp(startRot, endRot, rollSpeedCurve.Evaluate(p));
            yield return null;
        }
        rolling = false;
    }

    #endregion
}
