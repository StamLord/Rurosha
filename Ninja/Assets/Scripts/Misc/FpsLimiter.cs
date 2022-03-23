using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsLimiter : MonoBehaviour
{
    [SerializeField] private int frameRate = 60;
    void Start()
    {
        QualitySettings.vSyncCount = 0;
         Application.targetFrameRate = frameRate;
    }
}
