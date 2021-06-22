using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : Usable
{
    [SerializeField] private bool open;
    [SerializeField] private float slideDuration = .5f;
    [SerializeField] private float startSlide;

    [SerializeField] private Vector3 openPos;
    [SerializeField] private Vector3 closePos;
    

    public override void Use(Interactor interactor)
    {
        startSlide = Time.time;
        open = !open;
    }

    void Update()
    {
        float ratio = (Time.time - startSlide) / slideDuration;
        if(ratio <= 1f)
        {
            if(open)
                transform.localPosition = Vector3.Lerp(openPos, closePos, ratio);
            else
                transform.localPosition = Vector3.Lerp(closePos, openPos, ratio);
        }
    }
}
