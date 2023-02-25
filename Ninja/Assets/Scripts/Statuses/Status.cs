using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Status", order = 6)]
public class Status : ScriptableObject
{
    [SerializeField] private string statusName;
    [SerializeField] private string description;
    [SerializeField] private int cycles;
    [SerializeField] private float updateRate;
    [SerializeField] private int hpChange;
    [SerializeField] private int stChange;

    public string Name {get{return statusName;}}
    public string Description {get{return description;}}
    public int Cycles {get{return cycles;}}
    public float UpdateRate {get{return updateRate;}}
    public int HpChange {get{return hpChange;}}
    public int StChange {get{return stChange;}}
}
