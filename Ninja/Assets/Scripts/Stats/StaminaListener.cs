using UnityEngine;
using UnityEngine.Events;

public class StaminaListener : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private UnityEvent action;

    private void Start()
    {
        if(characterStats) characterStats.OnStaminaRestore += Execute;
    }

    private void Execute(float softStamina, float hardStamina)
    {
        action.Invoke();
    }
}
