using UnityEngine;

public class StatusComponent : MonoBehaviour
{   
    [SerializeField] private StatusManager manager;
    [SerializeField] private string statusName;
    [SerializeField] private string description;
    [SerializeField] private int cycles;
    [SerializeField] private float updateRate;
    
    [SerializeField] private int softHpChange;
    [SerializeField] private int hardHpChange;

    [SerializeField] private int softStChange;
    [SerializeField] private int hardStChange;

    [SerializeField] private float lastUpdate;
    [SerializeField] private int currentCycle = 0;

    public void Setup (StatusManager manager, string statusName, string description, int cycles, float updateRate, int hpChange, int stChange)
    {
        this.manager = manager;
        this.statusName = statusName;
        this.description = description;
        this.cycles = cycles;
        this.updateRate = updateRate;
        this.softHpChange = hpChange;
        this.softStChange = stChange;

        lastUpdate = Time.time;
    }

    private void Update()
    {
        // Update cycle
        if(Time.time - lastUpdate >= updateRate)
        {
            manager.StatusUpdate(softHpChange, hardHpChange, softStChange, hardStChange);

            lastUpdate = Time.time;
            currentCycle++;
        }

        // Destroy status after final cycle
        if(currentCycle >= cycles)
        {
            manager.RemoveStatus(statusName);
            
            // TODO: Pool object
            Destroy(this);
            return;
        }
    }
}
