using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class XmlManager : MonoBehaviour
{
    #region Singleton

    private static XmlManager _instance;
    public static XmlManager Instance { get{ return (_instance == null) ? _instance = FindXmlManager() : _instance;}}

    private static XmlManager FindXmlManager()
    {
        XmlManager dm = FindObjectOfType<XmlManager>();
        if(dm == null) 
        {
            GameObject go = new GameObject("XmlManager");
            dm = go.AddComponent<XmlManager>();
        }

        return dm;
    }

    private XmlManager SetupInstance()
    {
        if(_instance != null && _instance != this)
        {
            Debug.LogWarning("There is more than 1 XmlManager. Destroying object!");
            Destroy(gameObject);
        }
        else
            _instance = this;
        
        return _instance;
    }

    #endregion

    public void Serialize<T>(T xml, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        FileStream stream = new FileStream(Application.dataPath + filePath, FileMode.Create);
        serializer.Serialize(stream, xml);
        stream.Close();
    }

    public T Deserialize<T>(string filePath) where T : new()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        try
        {
            FileStream stream = new FileStream(Application.dataPath + filePath, FileMode.Open);
            T xmlWrapper = (T)serializer.Deserialize(stream);
            stream.Close();
            return xmlWrapper;
        }
        catch (FileNotFoundException)
        {
            Debug.LogError("[XmlManager] File not found: " + filePath);
        }
        catch
        {
            Debug.LogError("[XmlManager] Unknown error while loading file: " + filePath);
        }

        return new T();
    }
}