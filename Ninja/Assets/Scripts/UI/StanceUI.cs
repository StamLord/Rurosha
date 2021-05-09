using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StanceUI : MonoBehaviour
{
    [SerializeField] private Image low;
    [SerializeField] private Image medium;
    [SerializeField] private Image high;

    [SerializeField] Color selectedColor = new Color(1,1,1,1f);
    [SerializeField] Color baseColor = new Color(1,1,1,.5f);

    [SerializeField] private Katana katana;

    void Start()
    {
        katana.StanceSwitchStartEvent += ShowImages;
        katana.StanceSwitchDeltaEvent += UpdateImages;
        katana.StanceSwitchEndEvent += HideImages;
    }

    void ShowImages()
    {
        low.color = medium.color = high.color = baseColor;
    }

    void UpdateImages(Vector3 delta)
    {
        if(delta.y > 0.2f)
        { 
            low.color = baseColor;
            medium.color = baseColor;
            high.color = selectedColor;
        }
        else if(delta.y < -0.2f)
        {
            low.color = selectedColor;
            medium.color = baseColor;
            high.color = baseColor;
        }
        else
        {
            low.color = baseColor;
            medium.color = selectedColor;
            high.color = baseColor;
        }
    }

    void HideImages()
    {
        Color color = new Color(1,1,1,0f);
        low.color = medium.color = high.color = color;
    }
}
