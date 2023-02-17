using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperiencePopup : UIPopup
{
    [SerializeField] private CharacterStats stats;

    protected override void Initialize()
    {
        stats.OnAddExp += StartPopup;
    }
}
