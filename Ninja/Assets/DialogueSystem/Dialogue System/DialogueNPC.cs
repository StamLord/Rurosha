using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : IXMLSerializable
{
    public string NpcName {get; private set;}
    private Dictionary<string, string> topicResponses = new Dictionary<string, string>();

    #region Interaction

    private int _interactionsLeft = 4;
    public int InteractionsLeft {get {return _interactionsLeft;} set {_interactionsLeft = Mathf.Max(0, value);}}

    // Modifiers
    public float CharmMod {get; private set;}
    public float JokeMod {get; private set;}
    public float ThreatMod {get; private set;}
    public float BribeMod {get; private set;}

    // Responses to interaction
    private string CharmResponse;
    private string JokeResponse;
    private string ThreatResponse;
    private string BribeResponse;

    private string NegativeResponse = "I don't want to discuss this with someone like you.";

    #endregion

    public float LikesPlayer {get; private set;}  // 0 - 1 How likely to give answer
    public float FearsPlayer {get; private set;} // 0 - 1 How likely to give answer

    public DialogueNode DialogueNode {get; private set;}
    
    public DialogueNPC(string name, DialogueNode dialogueNode = null)
    {
        NpcName = name;
        DialogueNode = dialogueNode;

        CharmMod = Random.Range(.25f, 2f);
        JokeMod = Random.Range(.25f, 2f);
        ThreatMod = Random.Range(.25f, 2f);
        BribeMod = Random.Range(.25f, 2f);

        LikesPlayer = 1;
        FearsPlayer = 0;
    }

    #region Interactions

    public string Charm(float value)
    {
        InteractionsLeft--;
        LikesPlayer += value * CharmMod;
        LikesPlayer = Mathf.Clamp01(LikesPlayer);
        return CharmResponse;
    }

    public string Joke(float value)
    {
        InteractionsLeft--;
        LikesPlayer += value * JokeMod;
        LikesPlayer = Mathf.Clamp01(LikesPlayer);
        return JokeResponse;
    }

    public string Threat(float value)
    {   
        InteractionsLeft--;
        FearsPlayer += value * ThreatMod;
        LikesPlayer -= value;
        FearsPlayer = Mathf.Clamp01(FearsPlayer);
        LikesPlayer = Mathf.Clamp01(LikesPlayer);
        return ThreatResponse;
    }

    public string Bribe(float value)
    {
        InteractionsLeft--;
        LikesPlayer += value * BribeMod;
        LikesPlayer = Mathf.Clamp01(LikesPlayer);
        return BribeResponse;
    }

    #endregion

    // Decides if going to share information about topic
    public bool ShareInformation()
    {
        Debug.Log(LikesPlayer);
        Debug.Log(FearsPlayer);
        return (LikesPlayer > .7f || FearsPlayer > .5f);
    }

    /// <summary>
    /// Gets a response to a topic. First time response will be generated randomly and set as the future response.
    /// </summary>
    /// <param name="topic">The topic to get response for.</param>
    public string GetResponse(Topic topic)
    {
        // Check if NPC will share information
        if(ShareInformation() == false)
            return NegativeResponse;

        // If already responded stay consistent
        if(topicResponses.ContainsKey(topic.TopicName))
            return topicResponses[topic.TopicName];

        string response = string.Empty;

        // Generate random response
        if(topic != null) 
            response = topic.GetResponse();

        // Keep track of response for future consistency
        topicResponses[topic.TopicName] = response;
        return response;
    }

    public void Serialize(string fileName = null)
    {
        XmlManager.Instance.Serialize(GetXML(), "/Dialogues/" + NpcName + ".xml");
    }

    public void Deserialize(string fileName = null)
    {
        DialogueNpcXml xml = XmlManager.Instance.Deserialize<DialogueNpcXml>("/Dialogues/" + NpcName + ".xml");
        UnpackXML(xml);
    }

    private DialogueNpcXml GetXML()
    {
        DialogueNodeXML dialogueXml = DialogueNode.GetXML();
        return new DialogueNpcXml(NpcName, 
        CharmMod, JokeMod, ThreatMod, BribeMod,
        CharmResponse, JokeResponse, ThreatResponse, BribeResponse,
        NegativeResponse,
        topicResponses, dialogueXml);
    }

    private void UnpackXML(DialogueNpcXml xml)
    {
        NpcName = xml.NpcName;

        CharmMod = xml.CharmMod;
        JokeMod = xml.JokeMod;
        ThreatMod = xml.ThreatMod;
        BribeMod = xml.BribeMod;

        CharmResponse = xml.CharmResponse;
        JokeResponse = xml.JokeResponse;
        ThreatResponse = xml.ThreatResponse;
        BribeResponse = xml.BribeResponse;

        NegativeResponse = xml.NegativeResponse;

        topicResponses = new Dictionary<string, string>();
        for(int i = 0; i < xml.Topics.Count; i++)
            topicResponses[xml.Topics[i]] = xml.Responses[i];

        DialogueNode = DialogueNode.UnpackXML(xml.DialogueNode);
    }
}

// Wrapper class for XmlManager to handle
public class DialogueNpcXml
{
    public string NpcName;

    public float CharmMod;
    public float JokeMod;
    public float ThreatMod;
    public float BribeMod;

    public string CharmResponse;
    public string JokeResponse;
    public string ThreatResponse;
    public string BribeResponse;

    public string NegativeResponse;

    public List<string> Topics;
    public List<string> Responses;
    public DialogueNodeXML DialogueNode;

    public DialogueNpcXml(){}

    public DialogueNpcXml(string npcName, float charm, float joke, float threat, float bribe, string charmResponse, string jokeResponse, string threatResponse, string bribeResponse, string negativeResponse, Dictionary<string, string> topicResponses, DialogueNodeXML dialogeNodes)
    {
        NpcName = npcName;
        
        CharmMod = charm;
        JokeMod = joke;
        ThreatMod = threat;
        BribeMod = bribe;

        CharmResponse = charmResponse;
        JokeResponse = jokeResponse;
        ThreatResponse = threatResponse;
        BribeResponse = bribeResponse;

        NegativeResponse = negativeResponse;

        Topics = new List<string>(topicResponses.Keys);
        Responses = new List<string>(topicResponses.Values);
        DialogueNode = dialogeNodes;
    }
    
}