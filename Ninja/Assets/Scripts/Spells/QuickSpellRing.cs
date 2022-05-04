using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSpellRing : UIWindow
{
    [Header("References")]
    
    [SerializeField] private SpellManager spellManager;
    [SerializeField] private InputState inputState;
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform selectionImage;

    [Header("Ring Settings")]
    [SerializeField] private int selectionsNum = 8;
    [SerializeField] private float pressTimeToOpen = 1f;
    [SerializeField] private float minDistanceFromCenter = 10f;

    [Header("Slow Down")]
    [SerializeField] private float timeScale = .2f;

    private int selected;
    private bool visible;
    private float prevTimeScale;

    private void Update()
    {
        if(visible)
        {
            GetSelection();
            UpdateSelectionImage();
            if(inputState.Defend.State == VButtonState.PRESS_END)
                HideRing();
        }
        else
        {
            if(inputState.Defend.State == VButtonState.PRESS_END)
                spellManager.Cast(selected);
            if(inputState.Defend.State == VButtonState.PRESSED && inputState.Defend.PressTime > pressTimeToOpen)
                ShowRing();
        }
    }

    private void GetSelection()
    {
        Vector3 screenCenter = new Vector3(Screen.width * .5f, Screen.height * .5f, 0);
        Vector2 mousePos = Input.mousePosition - screenCenter;

        // Make sure we are far enough from center of screen to make any selection
        if(mousePos.magnitude < minDistanceFromCenter)
            return;
        
        float angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
        if(angle < 0)
             angle += 360;
        
        selected = Mathf.FloorToInt(angle / (360 / selectionsNum));
    }

    private void UpdateSelectionImage()
    {
        selectionImage.localRotation = Quaternion.Euler(0, 0, selected * -45);
    }

    private void ShowRing()
    {
        animator.Play("show");
        visible = true;
        UIManager.Instance.AddWindow(this, false, true, true);
        prevTimeScale = Time.timeScale;
        Time.timeScale = timeScale;
    }

    private void HideRing()
    {
        animator.Play("hide");
        visible = false;
        UIManager.Instance.RemoveWindow(this, false, true, true);
        Time.timeScale = prevTimeScale;
    }
}
