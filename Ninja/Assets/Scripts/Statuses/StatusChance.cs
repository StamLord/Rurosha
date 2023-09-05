using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatusChance
{
    public Status status;
    [Range(0,1f)] public float chance;

    public StatusChance(Status status, float chance = 1f)
    {
        this.status = status;
        this.chance = chance;
    }

    public bool Success(float modifier = 0f)
    {
        return Random.Range(0f, 1f) + modifier <= chance;
    }
}
