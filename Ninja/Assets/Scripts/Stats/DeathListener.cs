using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathListener : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private UnityEvent action;

    private void Start() 
    {
        if(characterStats) characterStats.OnDeath += Execute;
    }

    private void Execute()
    {
        action.Invoke();
    }
}