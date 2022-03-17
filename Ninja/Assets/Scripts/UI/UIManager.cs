using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour{}

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UIWindow> activeWindows = new List<UIWindow>();
    private bool _disabledMouse;
    private bool _disabledMovement;
    private bool _disabledInteraction;

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

    #region Events

    public delegate void DisableMovementDelegate();
    public static event DisableMovementDelegate OnDisableMovement;

    public delegate void EnableMovementDelegate();
    public static event EnableMovementDelegate OnEnableMovement;

    public delegate void DisableInteractionDelegate();
    public static event DisableInteractionDelegate OnDisableInteraction;

    public delegate void EnableInteractionDelegate();
    public static event EnableInteractionDelegate OnEnableInteraction;

    public delegate void DisableMouseDelegate();
    public static event DisableMouseDelegate OnDisableMouse;

    public delegate void EnableMouseDelegate();
    public static event EnableMouseDelegate OnEnableMouse;

    #endregion

    private void Awake() 
    {
        Debug.Log(gameObject.name);
        SetupInstance();
        CheckWindows();
    }
    
    public void AddWindow(UIWindow window, bool disableMovement = false, bool disableMouse = false, bool disableInteraction = false)
    {
        activeWindows.Add(window);
        if(disableMovement) EnableMovement(false);
        if(disableMouse) EnableMouse(false);
        if(disableInteraction) EnableInteraction(false);
        CheckWindows();
    }

    public void RemoveWindow(UIWindow window, bool enableMovement = false, bool enableMouse = false, bool enableInteraction = false)
    {
        activeWindows.Remove(window);
        if(enableMovement) EnableMovement(true);
        if(enableMouse) EnableMouse(true);
        if(enableInteraction) EnableInteraction(true);
        CheckWindows();
    }

    private void CheckWindows()
    {
        if(activeWindows.Count > 0)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void EnableMouse(bool state)
    {
        if(state) // Enable
        {
            _disabledMouse = false;
            if(OnEnableMouse != null) OnEnableMouse();
        }
        else // Disable
        {
            _disabledMouse = true;
            if(OnDisableMouse != null) OnDisableMouse();
        }
    }

    private void EnableMovement(bool state)
    {
        if(state) // Enable
        {
            _disabledMovement = false;
            if(OnEnableMovement != null) OnEnableMovement();
        }
        else // Disable
        {
            _disabledMovement = true;
            if(OnDisableMovement != null) OnDisableMovement();
        }
    }

    private void EnableInteraction(bool state)
    {
        if(state) // Enable
        {
            _disabledInteraction = false;
            if(OnEnableInteraction != null) OnEnableInteraction();
        }
        else // Disable
        {
            _disabledInteraction = true;
            if(OnDisableInteraction != null) OnDisableInteraction();
        }
    }
}
