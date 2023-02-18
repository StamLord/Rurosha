using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPopup : UIPopup
{
    [SerializeField] private CharacterStats stats;

    protected override void Initialize()
    {
        stats.OnMoneyUpdate += StartPopup;
    }
}
