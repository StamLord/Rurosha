using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunningRestore : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    [SerializeField] private Image image;
    [SerializeField] private Image target;
    [SerializeField] private float scaleDuration = 1f;

    [SerializeField] private float targetValue = .5f;
    [SerializeField] private float targetAllowance = .1f;

    [SerializeField] private float timingWindow = .2f;
    [SerializeField] private float softRestore = 10f;
    [SerializeField] private float hardRestore = 10f;

    private bool active;
    private float activeTime;
    private float upTime;
    [SerializeField] private bool validUp;

    private void Update()
    {
        CheckInput();
        UpdateImage();
    }

    private void UpdateImage()
    {
        float p = (Time.time - activeTime) / scaleDuration;
        image.rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, p);

        if(p >= 1)
            activeTime = Time.time;
    }

    private void CheckInput()
    {   
         if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            active = false;
            image.enabled = false;
            target.enabled = false;
            upTime = Time.time;

            validUp = (image.rectTransform.localScale.x <= targetValue &&
             targetValue - image.rectTransform.localScale.x <= targetAllowance);
        }
        
        //TODO: Switch to InputState
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            active = true;            
            activeTime = Time.time;
            image.enabled = true;
            target.enabled = true;

            if(validUp && Time.time - upTime <= timingWindow)
                Restore();
        }
    }

    private void Restore()
    {
        stats.RestoreStamina(softRestore, hardRestore);
    }
}
