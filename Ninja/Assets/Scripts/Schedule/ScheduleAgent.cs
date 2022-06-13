using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleAgent : MonoBehaviour
{
    [SerializeField] private Schedule schedule;
    [SerializeField] private DayNightManager dayNightManager;

    public Task GetCurrentTask()
    {
        float time = dayNightManager.GetTime();
        Task lastTask = new Task();

        foreach(Task t in schedule.Tasks)
        {
            // Convert task time to seconds
            float taskTime = t.hours * 60 * 60;
            taskTime += t.minutes * 60;

            // If task time has passed we reference to it
            if(time >= taskTime)
                lastTask = t;
            // First future task that is yet to start breaks the loop
            else
                break;
        }

        return lastTask;
    }
}
