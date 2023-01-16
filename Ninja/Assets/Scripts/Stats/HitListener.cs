using UnityEngine;
using UnityEngine.Events;


public class HitListener : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private UnityEvent action;

    private void Start()
    {
        if(characterStats) characterStats.OnHit += Execute;
    }

    private void Execute(int softDamage, int hardDamage)
    {
        action.Invoke();
    }
}
