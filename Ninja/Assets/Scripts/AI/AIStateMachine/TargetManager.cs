using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private List<FightAIState> activeFighters = new List<FightAIState>();
    [SerializeField] private List<FightAIState> pendingFighters = new List<FightAIState>();

    [SerializeField] private int maxActive;
    [SerializeField] private float timeBeforeNextAttacker = 2f;
    private float lastAttackTime;

    [SerializeField] private FightAIState lastAttacker;

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

    public bool ActiveToPending(FightAIState fighter)
    {
        if(activeFighters.Contains(fighter) == false)
            return false;

        // It should not be removed as it will be promoted again to active
        if(pendingFighters.Count == 0)
            return false;
        
        // Remove from active
        RemoveActive(fighter);

        // Add to pending
        pendingFighters.Add(fighter);

        return true;
    }

    private void RemovePending(FightAIState fighter)
    {
        pendingFighters.Remove(fighter);
    }

    private void RemoveActive(FightAIState fighter)
    {
        activeFighters.Remove(fighter);
        fighter.AllowAdvance(false);

        if(activeFighters.Count < maxActive)
            NextActive();

        if(lastAttacker == fighter)
            NextAttacker();
    }

    private void Update() 
    {
        if(Time.time - lastAttackTime > timeBeforeNextAttacker)
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
        {
            lastAttacker = f;
            f.AllowAttack(true);
        }
    }

    private void NextAttacker()
    {
        if(activeFighters.Count == 0)
            return;
        
        FightAIState f = activeFighters[0];
        if(lastAttacker) lastAttacker.AllowAttack(false);
        f.AllowAttack(true);
        lastAttacker = f;
    }

    public void FinishedAttack(FightAIState fighter)
    {
        lastAttackTime = Time.time;

        // Move to back of list
        activeFighters.Remove(fighter);
        activeFighters.Add(fighter);

        // Disable attack
        fighter.AllowAttack(false);

        // Activate next fighter
        NextAttacker();
    }
}
