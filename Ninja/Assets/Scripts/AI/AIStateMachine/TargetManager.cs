using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private List<FightAIState> activeFighters = new List<FightAIState>();
    [SerializeField] private List<FightAIState> pendingFighters = new List<FightAIState>();

    [SerializeField] private int maxActive;

    private FightAIState lastAttacker;

    public void AddFighter(FightAIState fighter)
    {
        // Don't add twice
        if(pendingFighters.Contains(fighter) || activeFighters.Contains(fighter)) return;
        
        // Add to pending
        pendingFighters.Add(fighter);

        // If below max active add next in line
        if(activeFighters.Count < maxActive)
            NextActive();
    }

    public void RemoveFighter(FightAIState fighter)
    {
        if(pendingFighters.Contains(fighter))
            RemovePending(fighter);

        if(activeFighters.Contains(fighter))
            RemoveActive(fighter);
    }

    private void RemovePending(FightAIState fighter)
    {
        if(pendingFighters.Contains(fighter))
            pendingFighters.Remove(fighter);
    }

    private void RemoveActive(FightAIState fighter)
    {
        if(activeFighters.Contains(fighter))
            activeFighters.Remove(fighter);

        if(activeFighters.Count < maxActive)
            NextActive();

        if(lastAttacker == fighter)
            NextAttacker();
    }

    private void NextActive()
    {
        // Need to have atleast one pending
        if(pendingFighters.Count < 1) return;

        // Get first in pending line and make active
        FightAIState f = pendingFighters[0];
        pendingFighters.RemoveAt(0);
        activeFighters.Add(f);
        f.AllowAdvance(true);
        
        // First one can attack
        if(activeFighters.Count == 1) 
            f.AllowAttack(true);
    }

    private void NextAttacker()
    {
        FightAIState f = activeFighters[0];
        f.AllowAttack(true);
        lastAttacker = f;
    }

    public void FinishedAttack(FightAIState fighter)
    {
        // Move to back of list
        activeFighters.Remove(fighter);
        activeFighters.Add(fighter);

        // Disable attack
        fighter.AllowAttack(false);

        // Activate next fighter
        NextAttacker();
    }
}
