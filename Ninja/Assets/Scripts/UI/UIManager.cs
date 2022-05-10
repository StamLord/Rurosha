using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public virtual void ProcessInput(Vector3 axis, bool select){}
    public virtual bool Select(int index){return false;}
    public virtual void Open(){}
    public virtual void Close(){}
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private InputState inputState;
    [SerializeField] private PlayerControls playerControls;
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
        SetupInstance();
        CheckWindows();
    }
    private void Update() 
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if(activeWindows.Count == 0)
            return;
        
        // Pass input only to last window opened
        UIWindow window = activeWindows[activeWindows.Count - 1];

        if(inputState.Num1.State == VButtonState.PRESS_START)
            window.Select(0);
        else if(inputState.Num2.State == VButtonState.PRESS_START)
            window.Select(1);
        else if(inputState.Num3.State == VButtonState.PRESS_START)
            window.Select(2);
        else if(inputState.Num4.State == VButtonState.PRESS_START)
            window.Select(3);
        else if(inputState.Num5.State == VButtonState.PRESS_START)
            window.Select(4);
        else if(inputState.Num6.State == VButtonState.PRESS_START)
            window.Select(5);
        else if(inputState.Num7.State == VButtonState.PRESS_START)
            window.Select(6);
        else if(inputState.Num8.State == VButtonState.PRESS_START)
            window.Select(7);
        else if(inputState.Num9.State == VButtonState.PRESS_START)
            window.Select(8);
        else if(inputState.Num0.State == VButtonState.PRESS_START)
            window.Select(9);
        else
            window.ProcessInput(inputState.AxisInput, inputState.Use.State == VButtonState.PRESS_START);
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
            playerControls.EnableMovement();
        }
        else // Disable
        {
            _disabledMovement = true;
            if(OnDisableMovement != null) OnDisableMovement();
            playerControls.DisableMovement();
        }
    }

    private void EnableInteraction(bool state)
    {
        if(state) // Enable
        {
            _disabledInteraction = false;
            if(OnEnableInteraction != null) OnEnableInteraction();
            playerControls.EnableInteraction();
        }
        else // Disable
        {
            _disabledInteraction = true;
            if(OnDisableInteraction != null) OnDisableInteraction();
            playerControls.DisableInteraction();
        }
    }
}
