using System.Collections.Generic;
using UnityEngine;

public class WorldDialogueManager : MonoBehaviour
{
    #region Singleton

    public static WorldDialogueManager instance;

    private void Awake() 
    {
        if(instance != null)
        {
            Debug.LogWarning("More than 1 instance of WorldDialogueManager exists. Destroying object" + gameObject.name);
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    
    #endregion

    private new Camera camera;

    private struct Message
    {
        public float startTime;
        public float duration;
        public string message;
        public Transform target;

        public Message(string message, float duration, Transform target)
        {
            this.message = message;
            this.duration = duration;
            this.target = target;
            startTime = Time.time;
        }
    }

    private List<Message> messages = new List<Message>();
    private Dictionary<Message, GameObject> messageObjects = new Dictionary<Message, GameObject>();

    public GameObject messagePrefab;
    public float verticalPixelOffset = 0f;
    public float verticalTransformOffset = 2f;

    public float minScale = .3f;
    public float maxScale = 1f;

    public float minDistance = 3f;
    public float maxDistance = 10f;

    public void NewMessage(string message, float duration, Transform target)
    {
        Message m = new Message(message, duration, target);
        messages.Add(m);
        messageObjects[m] = Instantiate(messagePrefab, transform);
    }

    private void UpdateMessages()
    {
        foreach(Message m in messages)
        {
            if(Time.time - m.startTime > m.duration)
                DeleteMessage(m);
            else
                UpdateMessagePosition(m);
        }
    }

    private void DeleteMessage(Message message)
    {
        messages.Remove(message);

        messageObjects[message].SetActive(false);
        messageObjects.Remove(message);
    }

    private void UpdateMessagePosition(Message message)
    {
        Vector3 screenPos = Vector3.zero;

        // Update position if within screen bounds
        if(WorldToScreen(message.target.position + Vector3.up * verticalTransformOffset, out screenPos))
        {
            messageObjects[message].SetActive(true);
            
            // Position
            messageObjects[message].transform.position = screenPos + Vector3.up * verticalPixelOffset;
            
            // Scale
            float clampedDistance = Mathf.Clamp(Vector3.Distance(camera.transform.position, message.target.position), minDistance, maxDistance);
            float distancePercentage = (clampedDistance - minDistance) / (maxDistance-minDistance);
            messageObjects[message].transform.localScale = Vector3.Lerp(Vector3.one * maxScale, Vector3.one * minScale, distancePercentage);
        }
        // Hide if outside of screen bounds
        else
        {
            messageObjects[message].SetActive(false);
        }
    }

    private bool WorldToScreen(Vector3 worldPosition, out Vector3 screenPosition)
    {   
        // Default
        screenPosition = Vector3.zero;

        if(camera == null)
            camera = Camera.main;

        // Check it's within screen bounds
        Vector3 viewportPos = camera.WorldToViewportPoint(worldPosition);
        if(viewportPos.x <= 0 || viewportPos.x >= 1 || viewportPos.y <= 0 || viewportPos.y >= 1 || viewportPos.z < 0) return false;

        // Get on-screen position
        screenPosition = camera.WorldToScreenPoint(worldPosition);
        return true;
    }

    private void Update() 
    {
        UpdateMessages();        
    }
}
