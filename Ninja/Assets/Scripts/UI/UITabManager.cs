using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITabManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tabs;
    [SerializeField] private int defaultTab;
    private int lastTab = 0;

    private void Start()
    {
        SwitchTab(defaultTab);
    }
    
    public void SwitchTab(int index)
    {
        if(index >= tabs.Length) 
            return;

        if(tabs[lastTab])
            tabs[lastTab].SetActive(false);
        
        if(tabs[index])
            tabs[index].SetActive(true);
        
        lastTab = index;
    }
}
