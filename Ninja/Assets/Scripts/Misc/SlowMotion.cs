using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [SerializeField] private float timeFactor = .25f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = timeFactor;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
    }
}
