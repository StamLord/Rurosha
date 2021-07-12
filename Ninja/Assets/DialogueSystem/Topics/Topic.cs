using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topic 
{
    protected string _topicName;
    protected string _question;
    protected string[] _positiveResponses;
    protected string[] _negativeResponses;

    public string TopicName {get{ return _topicName;}}

    public enum TopicKnowledgeLevel {COMMON, UNCOMMON, RARE}
    protected TopicKnowledgeLevel _knowledgeLevel;

    public Topic(){}

    public Topic(string topicName, string question, string[] positiveResponses, string[] negativeResponses, TopicKnowledgeLevel knowledgeLevel = TopicKnowledgeLevel.COMMON)
    {
        _topicName = topicName;
        _question = question;
        _positiveResponses = positiveResponses;
        _negativeResponses = negativeResponses;
        _knowledgeLevel = knowledgeLevel;
    }

    public string GetResponse()
    {
        bool positive = false;

        switch(_knowledgeLevel)
        {
            case TopicKnowledgeLevel.COMMON:
                positive = (Random.Range(1, 3) == 1); // 50% chance to know
                break;
            case TopicKnowledgeLevel.UNCOMMON:
                positive = (Random.Range(1, 6) == 1); // 20% chance to know
                break;
            case TopicKnowledgeLevel.RARE:
                positive = (Random.Range(1, 11) == 1); // 10% chance to know
                break;
        }

        return (positive) ? _positiveResponses[Random.Range(0, _positiveResponses.Length)] : _negativeResponses[Random.Range(0, _negativeResponses.Length)];
    }
}