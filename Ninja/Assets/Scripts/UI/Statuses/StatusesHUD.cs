using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusesHUD : MonoBehaviour
{
    [SerializeField] private StatusManager statusManager;
    [SerializeField] private Image[] icons = new Image[10];
    [SerializeField] private float blinkTime = 10;
    
    List<StatusComponent> statuses = new List<StatusComponent>();
    
    private void Start()
    {
        if(statusManager == null) return;

        statusManager.OnStatusStart += OnStatusUpdate;
        statusManager.OnStatusEnd += OnStatusUpdate;
    }

    private void OnStatusUpdate(string statusName)
    {
        statuses = statusManager.GetStatusComponents();

        UpdateImages();
    }

    private void UpdateImages()
    {
        int i = 0;

        for (; i < statuses.Count; i++)
        {
            icons[i].enabled = true;
            icons[i].sprite = statuses[i].status.Icon;
        }
        
        for(; i < icons.Length; i++)
            icons[i].enabled = false;
    }

    private void Update()
    {
        // Icons blink if they have less than <blinkTime> seconds left
        float sine = Mathf.Abs(Mathf.Sin(Time.time * 2));

        for (int i = 0; i < statuses.Count; i++)
        {
            Color color = icons[i].color;
            
            if(statuses[i].GetTimeLeft() <= blinkTime)
                color.a = sine;
            else
                color.a = 1f;
            
            icons[i].color = color;
        }
    }
}
