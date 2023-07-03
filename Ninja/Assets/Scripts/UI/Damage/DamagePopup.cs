using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : UIPopup
{
    public void Display(int amount)
    {
        StartPopup(amount);
    }
}
