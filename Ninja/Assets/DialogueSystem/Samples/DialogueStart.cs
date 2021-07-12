using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStart : MonoBehaviour
{
    void Start()
    {
        // Runtime
        // DialogueNode dn = new DialogueNode("My name is Ilan and I'm the fastest man alive.",
        // new List<string>() {
        //     "Hello Ilan.", 
        //     "Atlubu!",
        //     "Tov Ahi",
        //     "Bye now!"},
        // new List<DialogueNode>() {
        //     new DialogueNode("Shalom. Do you know of Castle Grayskull?", topics: new List<string> {"Castle Grayskull"}), 
        //     new DialogueNode("Yep"),
        //     new DialogueNode("Ma Tov Ahi?"),
        //     new DialogueNode("Goodbye")});

        // DialogueNPC npc = new DialogueNPC(1, "Ilan", dn);
        // npc.Serialize();
        // dn.Serialize(npc._npcName);

        // Load
        //DialogueNode dn = new DialogueNode("");
        //dn.Deserialize(npc.NpcName);
        DialogueNPC npc = new DialogueNPC("Ilan");
        npc.Deserialize();

        DialogueManager.Instance.StartDialogue(npc);
    }
}
