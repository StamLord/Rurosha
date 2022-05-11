using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitUI : MultipleChoiceWindow
{
    [Header("SitState Reference")]
    [SerializeField] private SitState sitState;

    private void Start() 
    {
        sitState.OnSitStart += Open;
        sitState.OnSitEnd += Close;
    }
}
