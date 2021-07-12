using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour{}

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UIWindow> activeWindows = new List<UIWindow>();

    #region Singleton

    private static UIManager _instance;
    public static UIManager Instance { get{ return (_instance == null) ? _instance = FindUIManager() : _instance;}}

    private static UIManager FindUIManager()
    {
        UIManager um = FindObjectOfType<UIManager>();
        if(um == null) 
        {
            GameObject go = new GameObject("UIManager");
            um = go.AddComponent<UIManager>();
        }

        return um;
    }

    private UIManager SetupInstance()
    {
        if(_instance != null && _instance != this)
        {
            Debug.LogWarning("There is more than 1 UIManager. Destroying object!");
            Destroy(gameObject);
        }
        else
            _instance = this;
        
        return _instance;
    }

    #endregion

    public delegate void DisableControlsDelegate();
    public static event DisableControlsDelegate OnDisableControls;

    public delegate void EnableControlsDelegate();
    public static event EnableControlsDelegate OnEnableControls;


    private void Awake() 
    {
        SetupInstance();    
    }
    
    public void AddWindow(UIWindow window)
    {
        activeWindows.Add(window);
        CheckWindows();
    }

    public void RemoveWindow(UIWindow window)
    {
        activeWindows.Remove(window);
        CheckWindows();
    }

    public void RemoveLastWindow()
    {
        activeWindows.RemoveAt(activeWindows.Count-1);
        CheckWindows();
    }

    private void CheckWindows()
    {
        if(activeWindows.Count > 0)
        {
            Cursor.lockState = CursorLockMode.None;
            if(OnDisableControls != null) OnDisableControls();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            if(OnEnableControls != null) OnEnableControls();
        }
    }
}
