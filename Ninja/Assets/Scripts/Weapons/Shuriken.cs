using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : WeaponObject
{
    [Header("References")]
    [SerializeField] private GameObject projectile;

    [Header("Throw Settings")]
    [SerializeField] private Vector3[] origins;
    [SerializeField] private float upModifier;
    [SerializeField] private float throwForce;
    [SerializeField] private int burstAmount = 3;
    [SerializeField] private float burstRange = 3f;
    [SerializeField] private float burstSpread = 45f;
    [SerializeField] private float lastShot;

    [Header("Aim Settings")]
    [SerializeField] private float aimTimeScale = .25f;
    [SerializeField] private float timeBeforeAimStart = .25f;
    private float aimStart;
    private bool aimCanceled;
    private bool isAiming;
    private int mode; // 0 Single, 1 Horizontal, 2 Vertical, 3 Cross

    [Header("Trajectory Settings")]
    [SerializeField] private bool displayTrajectory;
    [SerializeField] private LineRenderer[] lineRenderer;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private bool endOnCollision;
    [SerializeField] private int numPoints = 50;
    [SerializeField] private float timeBetweenPoints = .1f;

    [SerializeField] AttributeDependant<float> maxAngle = new AttributeDependant<float>("Dexterity", new float[]{35, 30, 25, 20, 16, 12, 8, 5, 3, 1});
    [SerializeField] AttributeDependant<float> shootRate = new AttributeDependant<float>("Agility", new float[]{2f, 1.75f, 1.5f, 1.25f, 1f, .8f, .6f, .5f, .4f, .35f});
    //[SerializeField] private float[] maxAnglePerDexterity = {35, 30, 25, 20, 16, 12, 8, 5, 3, 1};
    //[SerializeField] private float[] shootRatePerAgility = {2f, 1.75f, 1.5f, 1.25f, 1f, .8f, .6f, .5f, .4f, .35f};

    [Header("Exp")]
    [SerializeField] private float agilityExpGain = 1f;
    [SerializeField] private float dexterityExpGain = 2f;
    [SerializeField] private float dexterityExpMaxDistance = 20f;
    
    void Update()
    {
        switch (inputState.MouseButton1.State)
        {
            case VButtonState.PRESS_START:
                aimCanceled = false;
                if(isAiming == false)
                    aimStart = Time.time;
                break;
            case VButtonState.PRESSED:
                if(isAiming == false && 
                    aimCanceled == false && 
                    Time.time - aimStart >= timeBeforeAimStart)
                    StartAim();
                break;
            case VButtonState.PRESS_END:
                if(aimCanceled || Time.time < lastShot + shootRate.GetValue(manager.Stats))
                    return;

                float step = burstRange / (burstAmount - 1);

                switch(mode)
                {
                    case 0:
                        ThrowSingle();
                        break;
                    case 1:
                        ThrowMultiple(
                            camera.transform.position - camera.transform.right * burstRange *.5f, 
                            new Vector3(step, 0, 0));
                        break;
                    case 2:
                        ThrowMultiple(
                            camera.transform.position - camera.transform.up * burstRange *.5f, 
                            new Vector3(0, step, 0));
                        break;
                    case 3:
                        ThrowMultiple(
                            camera.transform.position - camera.transform.up * burstRange *.5f - camera.transform.right * burstRange *.5f, 
                            new Vector3(step, step, 0f));
                        ThrowMultiple(
                            camera.transform.position + camera.transform.up * burstRange *.5f - camera.transform.right * burstRange *.5f, 
                            new Vector3(step, -step, 0f));
                        break;
                }

                if(isAiming)
                    CancelAim();

                lastShot = Time.time;

                break;
        }

        switch (inputState.MouseButton2.State)
        {
            case VButtonState.PRESS_START:
                if(isAiming)
                    CancelAim();
                else
                    SwitchMode();
                break;
        }

        for (int i = 0; i < lineRenderer.Length; i++)
            if(lineRenderer[i].enabled)
                UpdateTrajectory(i);
    }

    private void ThrowSingle()
    {
        GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity);
        
        RaycastHit hit;
        Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 50f);
        
        float distance = 0;

        if(hit.collider)
        {
            obj.transform.forward = hit.point - transform.position;
            distance = Vector3.Distance(hit.point, camera.transform.position);
        }
        else
        {
            obj.transform.forward = camera.transform.forward;
            distance = dexterityExpMaxDistance;
        }

        float angle = maxAngle.GetValue(manager.Stats);
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(-angle, angle), Random.Range(-angle, angle));

        obj.transform.forward = randomRotation * obj.transform.forward;

        manager.DepleteItem(1);

        UseAnimation();

        distance = Mathf.Max(0.1f, distance / dexterityExpMaxDistance);
        manager.Stats.IncreaseAttributeExp("Dexterity", dexterityExpGain * distance);
        manager.Stats.IncreaseAttributeExp("Agility", agilityExpGain);
    }

    private void ThrowMultiple(Vector3 firstPos, Vector3 offset)
    {
        float angleStep = burstSpread / burstAmount;
        float firstAngleStep = 0 - angleStep * burstAmount / 2;

        int availableAmmo = Mathf.Min(burstAmount, manager.GetAmmo());

        for(int i = 0; i < availableAmmo; i++)
        {
            GameObject obj = Instantiate(projectile, firstPos + camera.transform.right * offset.x * i + camera.transform.up * offset.y * i, Quaternion.identity);
            obj.transform.forward = camera.transform.forward;//Quaternion.AngleAxis(firstAngleStep + angleStep * i, Vector3.up) * Camera.main.transform.forward;

            manager.DepleteItem(1);
        }
    }

    private void StartAim()
    {
        isAiming = true;
        Time.timeScale = aimTimeScale;
        ShowTrajectory(0, true);
    }

    private void CancelAim()
    {
        isAiming = false;
        aimCanceled = true;
        Time.timeScale = 1f;
        ShowTrajectory(0, false);
    }

    private void ShowTrajectory(int i, bool state)
    {
        if(displayTrajectory)
            lineRenderer[i].enabled = state;
    }

    private void UpdateTrajectory(int index)
    {
        lineRenderer[index].positionCount = numPoints;
        Vector3 startPos = transform.position + transform.right * origins[index].x + transform.up * origins[index].y + transform.forward * origins[index].z;
        Vector3 startVelocity = (camera.transform.forward + camera.transform.up * upModifier).normalized * throwForce;

        List<Vector3> points = new List<Vector3>();

        for (float i = 0; i < numPoints; i += timeBetweenPoints)
        {
            Vector3 p = startPos + startVelocity * i;
            p.y = startPos.y + startVelocity.y * i + Physics.gravity.y * .5f * i * i;
            points.Add(p);

            if(endOnCollision && Physics.OverlapSphere(p, .2f, collisionMask).Length > 0)
            {
                lineRenderer[index].positionCount = points.Count;
                break;
            }
        }

        lineRenderer[index].SetPositions(points.ToArray());
    }

    private void SwitchMode()
    {
        mode = (mode > 2)? 0 : mode + 1;
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        CursorType cursor = CursorType.SINGLE;

        switch(mode)
        {
            case 0:
                cursor = CursorType.SINGLE;
                break;
            case 1:
                cursor = CursorType.HORIZONTAL;
                break;
            case 2:
                cursor = CursorType.VERTICAL;
                break;
            case 3:
                cursor = CursorType.DIAGONAL_CROSS;
                break;
        }

        manager.SetCursor(cursor);
    }

    protected override void DrawWeapon()
    {
        base.DrawWeapon();
        UpdateCursor();
    }

    protected override void SheathWeapon()
    {
        base.SheathWeapon();
        manager.SetCursor(CursorType.SINGLE);
    }
}


