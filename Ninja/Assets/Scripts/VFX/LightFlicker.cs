using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private int smoothing = 5;

    private Queue<float> smoothQueue;
    private float totalSum;

    private void Start()
    {
        smoothQueue = new Queue<float>(smoothing);
    }

    private void OnValidate() 
    {
        Reset();    
    }

    private void Reset()
    {
        smoothQueue.Clear();
        totalSum = 0;
    }

    private void Update()
    {
        if(light == null) return;

        // Calculate new value
        float newValue = Random.Range(minIntensity, maxIntensity);
        totalSum += newValue;
        smoothQueue.Enqueue(newValue);
        
        // Remove the first value in queue if too big
        if(smoothQueue.Count > smoothing)
            totalSum -= smoothQueue.Dequeue();
        
        // Calculate and set average value
        light.intensity = totalSum / smoothQueue.Count;
    }
}
