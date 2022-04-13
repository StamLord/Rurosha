using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEntity : Usable
{
    [SerializeField] private string npcName;
    [SerializeField] private DialogueNPC dialogue;

    [SerializeField] private bool inDialogue;
    [SerializeField] private Transform talkingTo;

    [SerializeField] private float maxTalkingDistance = 2f;

    void Start()
    {
        dialogue = new DialogueNPC(npcName);
        dialogue.Deserialize();
    }

    public override void Use(Interactor interactor)
    {
        talkingTo = interactor.transform;
        DialogueManager.Instance.StartDialogue(dialogue);
        inDialogue = true;
    }

    private void Update() 
    {
        // Check distance of player
        if(talkingTo == null) return;
        if((transform.position - talkingTo.position).sqrMagnitude > maxTalkingDistance * maxTalkingDistance)
            StopDialogue();
    }

    private void StopDialogue()
    {
        talkingTo = null;
        DialogueManager.Instance.StopDialogue();
        inDialogue = false;
    }
}
