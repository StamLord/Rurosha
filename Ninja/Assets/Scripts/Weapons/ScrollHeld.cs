using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollHeld : WeaponObject
{
    [SerializeField] private Image content;

    protected override void UpdateVisual()
    {
        base.UpdateVisual();
        
        Scroll scroll = (Scroll)item;
        if(scroll)
        {
            content.sprite = scroll.content;
            content.color = Color.white;
        }
        else
            content.color = new Color(1,1,1,0); // Invisible
    }
}
