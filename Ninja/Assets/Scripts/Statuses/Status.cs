using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Status", order = 6)]
public class Status : ScriptableObject
{
    [SerializeField] private string statusName;
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;
    [SerializeField] private int cycles;
    [SerializeField] private float updateRate;
    [SerializeField] private int hpChange;
    [SerializeField] private int stChange;
    [SerializeField] private Status[] cures;
    [SerializeField] private Status[] prevents;

    public string Name {get{return statusName;}}
    public Sprite Icon {get{return icon;}}
    public string Description {get{return description;}}
    public int Cycles {get{return cycles;}}
    public float UpdateRate {get{return updateRate;}}
    public int HpChange {get{return hpChange;}}
    public int StChange {get{return stChange;}}
    public Status[] Cures {get{return cures;}}
    public Status[] Prevents {get{return prevents;}}
}
