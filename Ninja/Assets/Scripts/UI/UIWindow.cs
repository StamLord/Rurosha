using UnityEngine;

public class UIWindow : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] private bool closeOnBack;
    [SerializeField] private bool disableMovement;
    [SerializeField] private bool disableMouse;
    [SerializeField] private bool disableInteraction;

    private bool isOpen;
    public bool IsOpen {get {return isOpen;}}

    public virtual void ProcessInput(Vector3 axis, bool select){}
    public virtual bool Select(int index){return false;}
    
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
        else
            gameObject.SetActive(true);
        
        UIManager.Instance.AddWindow(this, disableMovement, disableMouse, disableInteraction);
    }

    public virtual void Close()
    {
        isOpen = false;
        if(animator)
            animator.Play("hide");
        else
            gameObject.SetActive(false);
        
        UIManager.Instance.RemoveWindow(this, disableMovement, disableMouse, disableInteraction);
    }
}