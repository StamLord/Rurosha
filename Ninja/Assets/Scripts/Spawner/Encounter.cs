using UnityEngine;

[System.Serializable]
public struct Encounter 
{
    public bool behindPlayer;
    public GameObject[] enemies;
    public float radius;
    public bool timeSensitive;
    public int hourStart;
    public int hourEnd;
}