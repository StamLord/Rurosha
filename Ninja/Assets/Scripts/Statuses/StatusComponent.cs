using UnityEngine;

public class StatusComponent : MonoBehaviour
{   
    public Status status;
    [SerializeField] private StatusManager manager;
    [SerializeField] private float lastUpdate;
    [SerializeField] private int currentCycle = 0;

    public float GetTimeLeft() => status.UpdateRate * status.Cycles - (status.UpdateRate * currentCycle + Time.time - lastUpdate);

    public void Setup (StatusManager manager, Status status)
    {
        this.manager = manager;
        this.status = status;

        lastUpdate = Time.time;
    }

    private void Update()
    {
        // Update cycle
        if(Time.time - lastUpdate >= status.UpdateRate)
        {
            manager.StatusUpdate(status.HpChange, 0, status.StChange, 0);

            lastUpdate = Time.time;
            currentCycle++;
        }

        // Destroy status after final cycle
        if(currentCycle >= status.Cycles)
        {
            manager.RemoveStatus(status.Name);
            
            // TODO: Pool object
            Destroy(this);
            return;
        }
    }
}
