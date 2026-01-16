using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem fire;

    public void SetFire(bool state)
    {
        if(state)
            fire.Play();
        else
            fire.Stop();
    }
}
