using UnityEngine;

public class TimeBased : MonoBehaviour
{
    [SerializeField] private DayNightManager.DayTime startTime;
    [SerializeField] private DayNightManager.DayTime endTime;
    [SerializeField] private bool state;

    private void Start()
    {
        // No reason to subscribe to both minute and hour events.
        // We subscribe to the more frequent of the two if needed.
        if(startTime.minutes == 0 && endTime.minutes == 0)
            DayNightManager.instance.OnHourPassed += TimeEvent;
        else
            DayNightManager.instance.OnMinutePassed += TimeEvent;

        // Initial update to make sure we match state value
        if(state)
            StartEvent();
        else
            EndEvent();
    }

    private void TimeEvent(DayNightManager.DayTime time)
    {
        bool startPassed = time >= startTime;
        bool endPassed = time >= endTime;

        // Example: 
        // Start: 01:00 End 03:00
        if(startTime < endTime)
        {
            if(startPassed)
            {
                if(endPassed)
                    EndEvent();
                else
                    StartEvent();
            }
            else
                EndEvent();
        }
        // Example:
        // Start: 17:00 End: 07:00
        else if (startTime > endTime)
        {
            if(endPassed)
            {
                if(startPassed)
                    StartEvent();
                else
                    EndEvent();
            }
            else
                StartEvent();
        }
    }

    private void StartEvent()
    {
        state = true;
        StartAction();
    }

    private void EndEvent()
    {
        state = false;
        EndAction();
    }

    [System.Obsolete("Use comparison operators on DayTime structs. Example: a > b, a <= b, etc.")]
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
        
    }

    protected virtual void EndAction()
    {
        
    }
}
