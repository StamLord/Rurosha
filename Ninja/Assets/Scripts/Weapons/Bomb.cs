using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : WeaponObject
{
    [Header("References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private new Transform camera;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private LineRenderer[] lineRenderer;
    [SerializeField] private int numPoints = 50;
    [SerializeField] private float timeBetweenPoints = .1f;

    [Header("Throw Settings")]
    [SerializeField] private Vector3[] origins;
    [SerializeField] private float upModifier;
    [SerializeField] private float throwForce;

    [Header("Trajectory Settings")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private bool endOnCollision;

    private bool isAimingSingle;
    private bool isAimingMultiple;

    void Update()
    {
        switch (inputState.MouseButton1.State)
        {
            case VButtonState.PRESS_START:
                if(isAimingMultiple)
                    CancelAimMultiple();
                else
                    StartAimSingle();
                break;
            case VButtonState.PRESS_END:
                if(isAimingSingle)
                {
                    ThrowBomb(0);
                    CancelAimSingle();
                }
                break;
        }

        switch (inputState.MouseButton2.State)
        {
            case VButtonState.PRESS_START:
                if(isAimingSingle)
                    CancelAimSingle();
                else
                    StartAimMultiple();
                break;
            case VButtonState.PRESS_END:
                if(isAimingMultiple)
                {
                    int ammo = weaponManager.GetAmmo();
                    for (int i = 0; i < ammo && i < origins.Length; i++)
                        ThrowBomb(i);
                    
                    CancelAimMultiple();
                }
                break;
        }

        for (int i = 0; i < lineRenderer.Length; i++)
        {
            if(lineRenderer[i].enabled)
                UpdateTrajectory(i);
        }
    }

    private void StartAimSingle()
    {
        isAimingSingle = true;
        ShowTrajectory(0, true);
    }

    private void CancelAimSingle()
    {
        isAimingSingle = false;
        ShowTrajectory(0, false);
    }

    private void StartAimMultiple()
    {
        isAimingMultiple = true;
        int ammo = weaponManager.GetAmmo();
        for(int i = 0; i < ammo && i < lineRenderer.Length; i++)
            ShowTrajectory(i, true);
    }

    private void CancelAimMultiple()
    {
        isAimingMultiple = false;
        for(int i = 0; i < lineRenderer.Length; i++)
            ShowTrajectory(i, false);
    }

    private void ThrowBomb(int index)
    {
        // Check ammo and deplete
        if(weaponManager.GetAmmo() < 1) return;
        weaponManager.DepleteItem(1);
        
        // Instantiate projectile
        Vector3 pos = transform.position + transform.right * origins[index].x + transform.up * origins[index].y + transform.forward * origins[index].z;
        GameObject obj = Instantiate(projectile, pos, camera.rotation);
        obj.GetComponent<Rigidbody>().AddForce((camera.forward + camera.up * upModifier).normalized * throwForce, ForceMode.VelocityChange);
    }

    private void ShowTrajectory(int i, bool state)
    {
        lineRenderer[i].enabled = state;
    }

    private void UpdateTrajectory(int index)
    {
        lineRenderer[index].positionCount = numPoints;
        Vector3 startPos = transform.position + transform.right * origins[index].x + transform.up * origins[index].y + transform.forward * origins[index].z;
        Vector3 startVelocity = (camera.forward + camera.up * upModifier).normalized * throwForce;

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
}
