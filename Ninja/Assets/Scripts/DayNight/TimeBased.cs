using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBased : MonoBehaviour
{
    [SerializeField] private DayNightManager.DayTime startTime;
    [SerializeField] private DayNightManager.DayTime endTime;
    [SerializeField] private bool state;

    void Start()
    {
        if(startTime.minutes == 0)
            DayNightManager.instance.OnHourPassed += StartEvent;
        else
            DayNightManager.instance.OnMinutePassed += StartEvent;

        if(endTime.minutes == 0)
            DayNightManager.instance.OnHourPassed += EndEvent;
        else
            DayNightManager.instance.OnMinutePassed += EndEvent;

        StartEvent(DayNightManager.instance.GetDayTime());
        EndEvent(DayNightManager.instance.GetDayTime());
    }

    private void StartEvent(DayNightManager.DayTime time)
    {
        // If already on, do nothing
        if(state) return;

        if(TimePassed(time, startTime))
        {
            if(TimePassed(time, endTime) == false)
            {
                state = true;
                StartAction();
            }
        }
    }

    private void EndEvent(DayNightManager.DayTime time)
    {
        // If already off, do nothing
        if(state == false) return;

        if(TimePassed(time, endTime))
        {
            state = false;
            EndAction();
        }
    }

     private bool TimePassed(DayNightManager.DayTime current, DayNightManager.DayTime target)
    {
        // If same hour and same minutes or bigger, perform action
        if(current.hours == target.hours && current.minutes >= target.minutes)
            return true;
        // Hour should be either bigger than start time or AM while start time is PM
        // For example: 
        // Start at 1AM, Currently it's 3AM - True
        // Start at 23 PM Currently it's 20 AM - False
        // Start at 23 PM Currently it's 1 AM - True
        else if(current.hours > target.hours || current.hours < 12 && target.hours > 12)
            return true;

        return false;
    }
    
    protected virtual void StartAction()
    {
        Debug.Log("Started");
    }

    protected virtual void EndAction()
    {
        Debug.Log("Ended");
    }
}
