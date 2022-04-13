using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadBar : MonoBehaviour
{
    [SerializeField] private Interactor interactor;
    [SerializeField] private Image image;

    void Start()
    {
        if(interactor)
            interactor.UpdateCarryTimerEvent += UpdateFill;
    }
    
    void UpdateFill(float percentage)
    {
        image.fillAmount = percentage;
    }
}

