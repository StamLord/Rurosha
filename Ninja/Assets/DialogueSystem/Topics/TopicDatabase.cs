using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopicDatabase : MonoBehaviour
{
    #region Singleton

    private static TopicDatabase _instance;
    public static TopicDatabase Instance { get{ return (_instance == null) ? _instance = FindTopicDatabase() : _instance;}}

    private static TopicDatabase FindTopicDatabase()
    {
        TopicDatabase td = FindObjectOfType<TopicDatabase>();
        if(td == null) 
        {
            GameObject go = new GameObject("TopicDatabase");
            td = go.AddComponent<TopicDatabase>();
        }

        return td;
    }

    private TopicDatabase SetupInstance()
    {
        if(_instance != null && _instance != this)
        {
            Debug.LogWarning("There is more than 1 TopicDatabase. Destroying object!");
            Destroy(gameObject);
        }
        else
            _instance = this;
        
        return _instance;
    }

    #endregion

    #region People

    private static Dictionary<string, Topic> _peopleTopics = new Dictionary<string, Topic>();
    private static Dictionary<string, Topic> _knownPeopleTopics = new Dictionary<string, Topic>();

    public static Dictionary<string, Topic> PeopleTopics {get {return _peopleTopics;}}
    public static Dictionary<string, Topic> KnownPeopleTopics {get {return _knownPeopleTopics;}}

    #endregion

    #region Locations

    private static Dictionary<string, Topic> _locationTopics = new Dictionary<string, Topic>();
    private static Dictionary<string, Topic> _knownLocationTopics = new Dictionary<string, Topic>();

    public static Dictionary<string, Topic> LocationTopics {get {return _locationTopics;}}
    public static Dictionary<string, Topic> KnownLocationTopics {get {return _knownLocationTopics;}}

    #endregion

    private void Awake() 
    {
        SetupInstance();
    }

    private void Start() 
    {
        _peopleTopics.Add("David", new Topic("David", "Where is David", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _peopleTopics.Add("Akira", new Topic("Akira", "Where is Akira", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _peopleTopics.Add("Nakamura", new Topic("Nakamura", "Where is Nakamura", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _peopleTopics.Add("Jin", new Topic("Jin", "Where is Jin", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _peopleTopics.Add("Tanaka", new Topic("Tanaka", "Where is Tanaka", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _peopleTopics.Add("Kanji", new Topic("Kanji", "Where is Kanji", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _peopleTopics.Add("Toriko", new Topic("Toriko", "Where is Toriko", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        //_topics.Add("Daniel", new Topic("Daniel", "Where is Daniel", new string[] {"On the couch"}));
        LearnTopic("David");
        LearnTopic("Akira");
        LearnTopic("Nakamura");
        LearnTopic("Jin");
        LearnTopic("Tanaka");
        LearnTopic("Kanji");
        LearnTopic("Toriko");

        _locationTopics.Add("Castle Grayskull", new Topic("Castle Grayskull", "Where is Castle Grayskull", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _locationTopics.Add("Eternia", new Topic("Eternia", "Where is Eternia", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _locationTopics.Add("Mountain Fuji", new Topic("Mountain Fuji", "Where is Mountain Fuji", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));
        _locationTopics.Add("Zion", new Topic("Zion", "Where is Zion", new string[] {"Located near X", "I seen them over at X"}, new string[] {"I have no idea", "Never heard of him"}));

        //LearnTopic("Castle Grayskull");
        LearnTopic("Eternia");
        LearnTopic("Mountain Fuji");
        LearnTopic("Zion");
    }

    public static bool PlayerKnowsTopic(string topic, out Dictionary<string, Topic> topicsCollection)
    {
        bool topicKnown = false;
        
        topicsCollection = null;
        
        if(_knownPeopleTopics.ContainsKey(topic))
        {
            topicKnown = true;
            topicsCollection = _knownPeopleTopics;
        }

        if(_knownLocationTopics.ContainsKey(topic))
        {
            topicKnown = true;
            topicsCollection = _knownLocationTopics;
        }
        
        return topicKnown;
    }

    public static Topic GetTopic(string topic)
    {
        Dictionary<string, Topic> topicsCollection = null;
        if(TopicExists(topic, out topicsCollection))
            return topicsCollection[topic];

        return null;
    }

    public static bool TopicExists(string topic, out Dictionary<string, Topic> topicsCollection)
    {
        bool topicExists = false;
        
        topicsCollection = null;
        
        if(_peopleTopics.ContainsKey(topic))
        {
            topicExists = true;
            topicsCollection = _peopleTopics;
        }

        if(_locationTopics.ContainsKey(topic))
        {
            topicExists = true;
            topicsCollection = _locationTopics;
        }
        
        return topicExists;
    }

    public static bool LearnTopic(string topic)
    {
        Dictionary<string, Topic> topicsCollection;
        
        if(PlayerKnowsTopic(topic, out topicsCollection)) return true;

        if(TopicExists(topic, out topicsCollection) == false)
            return false;

        if(topicsCollection == _peopleTopics)
            _knownPeopleTopics.Add(topic, _peopleTopics[topic]);

        if(topicsCollection == _locationTopics)
            _knownLocationTopics.Add(topic, _locationTopics[topic]);

        return true;
    }

    public static List<string> GetKnownPeopleTopics()
    {
        return GetTopicList(_knownPeopleTopics);
    }

    public static List<string> GetKnownLocationTopics()
    {
        return GetTopicList(_knownLocationTopics);
    }

    private static List<string> GetTopicList(Dictionary<string, Topic> topicCollection)
    {
        Dictionary<string, Topic>.KeyCollection keys = topicCollection.Keys;
        List<string> topics = new List<string>();

        foreach (string key in keys)
            topics.Add(key);
        
        return topics;
    }
}