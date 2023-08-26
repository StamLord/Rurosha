using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    private Dictionary<string, StatusComponent> statusComponents = new Dictionary<string, StatusComponent>();

    // Events
    public delegate void StatusStartDelegate(string statusName);
    public event StatusStartDelegate OnStatusStart;

    public delegate void StatusUpdateDelegate(int softHp, int hardHp, int softSt, int hardSt);
    public event StatusUpdateDelegate OnStatusUpdate;

    public delegate void StatusEndDelegate(string statusName);
    public event StatusEndDelegate OnStatusEnd;

    public void AddStatus(Status status)
    {
        // Don't add the same status twice
        if(statusComponents.ContainsKey(status.Name))
            return;
        
        Debug.Log(gameObject.name + " got afflicted with status: " + status.Name);

        // Add status component to our gameObject. 
        // The component is responsible for updating us and destorying itself when time is over
        StatusComponent comp = gameObject.AddComponent<StatusComponent>();
        comp.Setup(this, status);

        // We track the components in a dictionary
        statusComponents.Add(status.name, comp);

        if(OnStatusStart != null)
            OnStatusStart(status.Name);
    }

    public void StatusUpdate(int softHp, int hardHp, int softSt, int hardSt)
    {
        if(OnStatusUpdate != null)
            OnStatusUpdate(softHp, hardHp, softSt, hardSt);
    }

    public void RemoveStatus(string statusName)
    {
        statusComponents.Remove(statusName);

        if(OnStatusEnd != null)
            OnStatusEnd(statusName);
    }

    public List<StatusComponent> GetStatusComponents()
    {
        return new List<StatusComponent>(statusComponents.Values);
    }
}
