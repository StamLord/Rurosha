using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : WeaponObject, IHitboxResponder
{
    [Header("References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private new Transform camera;
    [SerializeField] private Transform origin;
    [SerializeField] private GameObject arrow;
    [SerializeField] private List<GameObject> arrows = new List<GameObject>();
    [SerializeField] private float yArrowOffset = .1f;
    [SerializeField] private bool arrowsConverge;
    [SerializeField] private float arrowSpread = 30f;

    [Space(20)]

    [Header("String")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform stringTop;
    [SerializeField] private Transform stringBot;
    
    [Space(20)]

    [Header("Settings")]
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private int maxLoadedArrows = 1;
    [SerializeField] private float raycastDistance = 50;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float focusTimeScale = .3f;

    [Space(20)]

    [Header("Info")]
    [SerializeField] private bool isAiming;
    [SerializeField] private int loadedArrows;
    [SerializeField] private float charge;
    [SerializeField] private bool isFocus;

    private void OnValidate()
    {
        UpdateString();
    }

    private void Update()
    {
        ProcessInput();
        UpdateCharge();
        UpdateAim();
    }

    private void LateUpdate() 
    {
        UpdateString();
    }

    private  void UpdateCharge()
    {
        if(isAiming)
        {
            charge += Time.deltaTime;
            if(charge > maxChargeTime) 
                charge = maxChargeTime;
        }
        else
            charge = 0;

        animator.SetFloat("CHARGE", charge);
    }

    private void ProcessInput()
    {
        if(inputState.Defend.State == VButtonState.PRESS_START)
            LoadArrow();

        switch (inputState.MouseButton1.State)
        {
            case VButtonState.PRESS_START:
                if(isAiming == false && loadedArrows > 0)
                    StartAim();
                else if(loadedArrows == 0)
                    animator.SetTrigger("LMB");
                break;
            case VButtonState.PRESS_END:
                if(isAiming)
                    Shoot();
                break;
        }

        switch(inputState.MouseButton2.State)
        {
            case VButtonState.PRESS_START:
                if(isFocus == false && loadedArrows > 0)
                    StartFocus();
                else if(loadedArrows == 0)
                    animator.SetTrigger("RMB");
                break;
            case VButtonState.PRESS_END:
                if(isFocus)
                    StopFocus();
                break;
        }
    }

    private void StartAim()
    {
        isAiming = true;
        animator.SetBool("AIM", isAiming);
    }

    private void CancelAim()
    {   
        isAiming = false;
        animator.SetBool("AIM", isAiming);
    }

    private void LoadArrow()
    {   
        // Do nothing if max arrows loaded
        if(loadedArrows >= maxLoadedArrows)
            return;

        // Check if enough arrows in weaponManager
        if(manager.RemoveAmmo("Arrow") == false) 
            return;
        
        loadedArrows++;

        // Update visual arrow objects 
        UpdateVisualArrows();
    }

    private void UpdateVisualArrows()
    {
        // Instantiate arrows if not enough
        int delta = loadedArrows - arrows.Count;
        if(delta > 0)
        {
            for(int i = 0; i < delta; i++)
                arrows.Add(Instantiate(arrow, origin));
        }
        
        float midPoint = loadedArrows * yArrowOffset * .5f;

        // Activate and reposition arrows
        for(int i = 0; i < arrows.Count; i++)
        {
            bool active = (i < loadedArrows);
            arrows[i].SetActive(active);
            if(active)
            {
                arrows[i].transform.position = origin.position + origin.up * (i * yArrowOffset - midPoint);
                arrows[i].transform.localEulerAngles = Vector3.zero;
            }
        }
    }

    private void UpdateAim() 
    {
        if(isAiming == false) return;

        // Get aim target
        RaycastHit hit;
        bool aimHit = Physics.Raycast(camera.position, camera.forward, out hit, raycastDistance, raycastMask);
        Debug.DrawRay(camera.position, camera.forward * raycastDistance, Color.green);

        float angleStep = arrowSpread / (loadedArrows - 1);
        float firstAngle = arrowSpread / 2;

        // Aim arrows at target or spread
        int activeIndex = 0;
        for(int i = 0; i < arrows.Count; i++)
        {
            // Skip inactive
            if(arrows[i].activeSelf == false) continue;

            // Rotate towards target
            if(arrowsConverge && aimHit || loadedArrows == 1 && aimHit)
            {    
                Quaternion rot = Quaternion.LookRotation(hit.point - arrows[i].transform.position, Vector3.up);
                arrows[i].transform.rotation = rot;
            }
            else 
            {
                Vector3 eulerAngles = new Vector3(firstAngle - angleStep * activeIndex, 0, 0);
                arrows[i].transform.localEulerAngles = eulerAngles;
            }

            activeIndex++;
        }
    }

    private void Shoot()
    {
        // Play animation
        animator.Play("shoot");

        // Instantiate projectile
        if(projectile != null)
        {
            foreach(GameObject arr in arrows)
            {
                if(arr.activeSelf == false) continue;
                Projectile proj = Instantiate(projectile, arr.transform.position, arr.transform.rotation).GetComponent<Projectile>();

                // Set this transform's root to be ignored by the projectile
                if(proj)
                    proj.SetIgnoreTransform(transform.root);
            }
        }

        // Remove arrows from ammo in weaponmanager
        loadedArrows = 0;
        UpdateVisualArrows();
        
        // Reset aiming flags
        CancelAim();
    }

    private void StartFocus()
    {
        isFocus = true;
        Time.timeScale = focusTimeScale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    private void StopFocus()
    {
        isFocus = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    private void UpdateString()
    {
        lineRenderer.SetPosition(0, stringTop.position);
        lineRenderer.SetPosition(1, origin.position);
        lineRenderer.SetPosition(2, stringBot.position);
    }

    public void CollisionWith(Collider col, Hitbox hitbox)
    {

    }

    public void UpdateColliderState(bool state)
    {

    }

    public void GuardedBy(Collider collider, Hitbox hitbox)
    {
        // Play guarded animation
        // Depelte stamina
        // Stun if run out of stamina
    }
}
