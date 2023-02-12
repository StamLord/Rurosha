using UnityEngine;

public class UIWindow : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] protected Animator animator;

    [Header("Settings")]
    [SerializeField] private bool closeOnBack;
    [SerializeField] private bool disableMovement;
    [SerializeField] private bool disableMouse;
    [SerializeField] private bool disableInteraction;

    [Header("Associated Key")]
    [SerializeField] private bool openWithKey;
    [SerializeField] private KeyCode key;

    [Header("Container")]
    [SerializeField] private GameObject container;

    private bool isOpen;
    public bool IsOpen {get {return isOpen;}}

    public virtual void ProcessInput(Vector3 axis, bool select){}
    public virtual bool Select(int index){return false;}
    
    private void Update() 
    {
        if(openWithKey && Input.GetKeyDown(key))
        {
            if(IsOpen)
                Close();
            else
                Open();
        }
    }

    public virtual void Back() 
    {
        if(closeOnBack)
            Close();
    }
    
    public virtual void Open()
    {
        isOpen = true;
        if(animator)
            animator.Play("show");
        else if(container)
            container.SetActive(true);
        
        UIManager.Instance.AddWindow(this, disableMovement, disableMouse, disableInteraction);
    }

    public virtual void Close()
    {
        isOpen = false;
        if(animator)
            animator.Play("hide");
        else if(container)
            container.SetActive(false);
        
        UIManager.Instance.RemoveWindow(this, disableMovement, disableMouse, disableInteraction);
    }
}