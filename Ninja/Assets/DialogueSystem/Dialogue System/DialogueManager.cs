using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    #region Singleton

    private static DialogueManager _instance;
    public static DialogueManager Instance { get{ return (_instance == null) ? _instance = FindDialogueManager() : _instance;}}

    private static DialogueManager FindDialogueManager()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        if(dm == null) 
        {
            GameObject go = new GameObject("DialogueManager");
            dm = go.AddComponent<DialogueManager>();
        }

        return dm;
    }

    private DialogueManager SetupInstance()
    {
        if(_instance != null && _instance != this)
        {
            Debug.LogWarning("There is more than 1 DialogueManager. Destroying object!");
            Destroy(gameObject);
        }
        else
            _instance = this;
        
        return _instance;
    }

    #endregion

    #region Self

    private bool _inDialogue;
    public bool InDialogue { get {return _inDialogue;}}
    private DialogueNPC _npc;
    private DialogueNode _rootNode;
    private DialogueNode _currentNode;
    
    [SerializeField] private List<string> dialogueLog = new List<string>();

    const string playerNamePattern = @"\$PlayerName";
    const string appearancePattern = @"\$Appearance";
    const string reputationPattern = @"\$Reputation";
    
    #endregion

    void Awake()
    {
        SetupInstance();
    }

    public void StartDialogue(DialogueNPC npc)
    {
        _inDialogue = true;
        _npc = npc;
        _rootNode = npc.DialogueNode;
        
        UIDialogueManager.Instance.UpdateBars(_npc.LikesPlayer, _npc.FearsPlayer);
        UIDialogueManager.Instance.SetDialogueWindow(true);
        
        SetNode(_rootNode);
        if(_npc.InteractionsLeft > 0) UIDialogueManager.Instance.SetActions(true);

        Cursor.lockState = CursorLockMode.None;            
    }

    // Creates the Ask node that contains all questions about known topics
    private DialogueNode SetupAskNode()
    {   
        // Get all known people
        List<string> knownPeopleTopics = TopicDatabase.GetKnownPeopleTopics();
        List<DialogueNode> peopleDialogueNodes = new List<DialogueNode>();

        foreach (string topic in knownPeopleTopics)
        {
            peopleDialogueNodes.Add(new DialogueNode(_npc.GetResponse(TopicDatabase.KnownPeopleTopics[topic])));
        }

        DialogueNode peopleNode = new DialogueNode("Who do you want to know about?", knownPeopleTopics, peopleDialogueNodes);

        // Get all known locations
        List<string> knownLocationTopics = TopicDatabase.GetKnownLocationTopics();
        List<DialogueNode> locationDialogueNodes = new List<DialogueNode>();

        foreach (string topic in knownLocationTopics)
        {
            locationDialogueNodes.Add(new DialogueNode(_npc.GetResponse(TopicDatabase.KnownLocationTopics[topic])));
        }

        DialogueNode locationNode = new DialogueNode("What location do you want to know about?", knownLocationTopics, locationDialogueNodes);

        return new DialogueNode("How can I help you?", new List<string>() {"Topics", "Places"}, new List<DialogueNode>() {peopleNode, locationNode});
    }

    private void SetNode(DialogueNode node)
    {
        dialogueLog.Add(node.TextMessage);

        // Learn topics
        if(node.Topics != null)
        {
            foreach(string t in node.Topics)
                TopicDatabase.LearnTopic(t);
        }

        bool drawRootChoices = false;

        // If node is a dead end or the root node, we need to show choices from root node
        if(node.Choices == null || node.Choices.Count == 0 || node == _rootNode)
            drawRootChoices = true;

        List<string> choices = node.Choices;
        _currentNode = node;

        // If showing choices from root node, we add "Ask" choice
        if(drawRootChoices)
        {
            choices = new List<string>(_rootNode.Choices);
            choices.Add("Ask about...");
            _currentNode = _rootNode; // Needed in case we are in a dead end node and are showing root node choices
        }

        UIDialogueManager.Instance.DrawNode(PrepareTextMessage(node.TextMessage), choices, _npc.NpcName);
    }

    // Called on choice selection by UI element
    public void SelectChoice(int index)
    {   
        // If choice is outside of bounds, select Ask
        if(index >= _currentNode.Nodes.Count)
            SelectAsk();
        else
        {
            dialogueLog.Add(_currentNode.Choices[index]);
            SetNode(_currentNode.Nodes[index]);
        }
    }

    // Called when selecting the Ask option - Technically SelectChoice calls this when index is outside of bounds
    public void SelectAsk()
    {
        SetNode(SetupAskNode());
    }

    public void StopDialogue()
    {
        _inDialogue = false;
        _rootNode = null;
        _currentNode = null;
        UIDialogueManager.Instance.ClearNode();
        UIDialogueManager.Instance.SetDialogueWindow(false);
        Cursor.lockState = CursorLockMode.Locked;  
    }

    public void Charm()
    {
        SetNode(new DialogueNode(_npc.Charm(.2f)));
        if(_npc.InteractionsLeft < 1) UIDialogueManager.Instance.SetActions(false);
        UIDialogueManager.Instance.UpdateBars(_npc.LikesPlayer, _npc.FearsPlayer);
    }

    public void Joke()
    {
        SetNode(new DialogueNode(_npc.Joke(.2f)));
        if(_npc.InteractionsLeft < 1) UIDialogueManager.Instance.SetActions(false);
        UIDialogueManager.Instance.UpdateBars(_npc.LikesPlayer, _npc.FearsPlayer);
    }

    public void Threat()
    {
        SetNode(new DialogueNode(_npc.Threat(.2f)));
        if(_npc.InteractionsLeft < 1) UIDialogueManager.Instance.SetActions(false);
        UIDialogueManager.Instance.UpdateBars(_npc.LikesPlayer, _npc.FearsPlayer);
    }

    public void Bribe(int amount)
    {
        SetNode(new DialogueNode(_npc.Bribe(.2f)));
        if(_npc.InteractionsLeft < 1) UIDialogueManager.Instance.SetActions(false);
        UIDialogueManager.Instance.UpdateBars(_npc.LikesPlayer, _npc.FearsPlayer);
    }

    private string PrepareTextMessage(string textMessage)
    {
        // Replace special tags
        textMessage = Regex.Replace(textMessage, playerNamePattern, "Player");
        textMessage = Regex.Replace(textMessage, appearancePattern, "Samurai");
        textMessage = Regex.Replace(textMessage, reputationPattern, "Renowned");
        
        return textMessage;
    }
}