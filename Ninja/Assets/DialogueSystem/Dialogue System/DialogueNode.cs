using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode : IXMLSerializable
{
    public string TextMessage { get; private set; }
    public List<string> Choices { get; private set; }
    public List<DialogueNode> Nodes { get; private set; }
    public List<string> Topics { get; private set; }

    public DialogueNode(string textMessage, List<string> choices = null, List<DialogueNode> nodes = null, List<string> topics = null)
    {
        TextMessage = textMessage;
        Choices = (choices != null) ? choices : new List<string>();
        Nodes = (nodes != null) ? nodes : new List<DialogueNode>();
        Topics = (topics != null) ? topics : new List<string>();
    }

    public bool GetNode(int index, out DialogueNode node)
    {
        if(index > Nodes.Count - 1)
        {
            node = null;
            return false;
        }

        node = Nodes[index];
        return true;
    }

    public void AddChoice(string choiceText, DialogueNode node)
    {
        Choices.Add(choiceText);
        Nodes.Add(node);
    }

    // Saves the node to a file
    public void Serialize(string npcName)
    {
        XmlManager.Instance.Serialize(GetXML(), "/Dialogues/" + npcName + ".xml");
    }

    // Loads the node from a file
    public void Deserialize(string npcName)
    {
        DialogueNodeXML xml = XmlManager.Instance.Deserialize<DialogueNodeXML>("/Dialogues/" + npcName + ".xml");
        DialogueNode unpacked = UnpackXML(xml);

        TextMessage = unpacked.TextMessage;
        Choices = unpacked.Choices;
        Nodes = unpacked.Nodes;
        Topics = unpacked.Topics;
    }

    // Creates an XML wrapper class for XmlManager to serialize
    public DialogueNodeXML GetXML()
    {
        DialogueNodeXML xml = new DialogueNodeXML();
        xml.TextMessage = TextMessage; Debug.Log(TextMessage);
        xml.Choices = Choices;
        xml.Topics = Topics;
        
        List<DialogueNodeXML> xmlNodes = new List<DialogueNodeXML>();
        foreach(DialogueNode node in Nodes)
            xmlNodes.Add(node.GetXML());

        xml.Nodes = xmlNodes;

        return xml;
    }

    /// Unpacks an XML wrapper class to fill the class
    public static DialogueNode UnpackXML(DialogueNodeXML xml)
    {
        List<DialogueNode> nodes = new List<DialogueNode>();
        
        if(xml.Nodes != null)
            foreach(DialogueNodeXML xmlNode in xml.Nodes)
                nodes.Add(UnpackXML(xmlNode));

        return new DialogueNode(xml.TextMessage, xml.Choices, nodes, xml.Topics);;
    }
}

public class DialogueNodeXML
{
    public string TextMessage;
    public List<string> Choices;
    public List<DialogueNodeXML> Nodes;
    public List<string> Topics;
}