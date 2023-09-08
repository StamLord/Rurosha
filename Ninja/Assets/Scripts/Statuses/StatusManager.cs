using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    private Dictionary<string, StatusComponent> statusComponents = new Dictionary<string, StatusComponent>();

    private Dictionary<string, ParticleSystem> statusParticleDict = new Dictionary<string, ParticleSystem>();

    [SerializeField] private string[] statusParticleKeys;
    [SerializeField] private ParticleSystem[] statusParticleValues;

    // Events
    public delegate void StatusStartDelegate(Status status);
    public event StatusStartDelegate OnStatusStart;

    public delegate void StatusUpdateDelegate(int softHp, int hardHp, int softSt, int hardSt);
    public event StatusUpdateDelegate OnStatusUpdate;

    public delegate void StatusEndDelegate(Status status);
    public event StatusEndDelegate OnStatusEnd;

    private void Awake()
    {
        if(statusParticleKeys == null || statusParticleValues == null) return;
        
        int min = statusParticleKeys.Length;
        if(statusParticleKeys.Length != statusParticleValues.Length)
        {
            Debug.LogWarning("statusParticleKeys and statusParticleValues are not the same size! Keys: " + statusParticleKeys.Length + " Values: " + statusParticleValues.Length);
            min = Mathf.Min(statusParticleKeys.Length, statusParticleValues.Length);
        }

        for (int i = 0; i < min; i++)
            statusParticleDict.Add(statusParticleKeys[i], statusParticleValues[i]);
    }

    public void AddStatus(Status status)
    {
        // Don't add the same status twice
        if(statusComponents.ContainsKey(status.Name))
            return;

        // Make sure we don't have a status preventing the new status
        foreach (String statusName in statusComponents.Keys)
        {
            Status iterating = statusComponents[statusName].status;
            foreach (Status prevent in iterating.Prevents)
            {
                if(prevent.Name == status.Name)
                    return;
            }
        }
        
        Debug.Log(gameObject.name + " got afflicted with status: " + status.Name);

        // Add status component to our gameObject. 
        // The component is responsible for updating us and destorying itself when time is over
        StatusComponent comp = gameObject.AddComponent<StatusComponent>();
        comp.Setup(this, status);

        // Cure statuses
        foreach (Status cure in status.Cures)
            RemoveStatus(cure.Name);

        // We track the components in a dictionary
        statusComponents.Add(status.name, comp);

        // Activate vfx
        if(statusParticleDict.ContainsKey(status.Name))
            statusParticleDict[status.Name].Play();

        if(OnStatusStart != null)
            OnStatusStart(status);
    }

    public void StatusUpdate(int softHp, int hardHp, int softSt, int hardSt)
    {
        if(OnStatusUpdate != null)
            OnStatusUpdate(softHp, hardHp, softSt, hardSt);
    }

    public void RemoveStatus(string statusName)
    {
        bool found = statusComponents.ContainsKey(statusName);
        
        if(found == false) return;

        if(OnStatusEnd != null)
            OnStatusEnd(statusComponents[statusName].status);
        
        statusComponents.Remove(statusName);

        // Activate vfx
        if(statusParticleDict.ContainsKey(statusName))
            statusParticleDict[statusName].Stop();
    }

    public List<StatusComponent> GetStatusComponents()
    {
        return new List<StatusComponent>(statusComponents.Values);
    }
}
