using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Schedule
{
    [SerializeField] private List<Task> tasks = new List<Task>();
    public List<Task> Tasks {get{return tasks;}}
}

[System.Serializable]
public struct Task
{
    public float hours;
    public float minutes;
    public string location;

    public Task (string location = "", float hours = 0f, float minutes = 0f)
    {
        this.location = location;
        this.hours = hours;
        this.minutes = minutes;
    }
}
