using UnityEngine;

public class UIWindow : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] private bool closeOnBack;

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
        UIManager.Instance.AddWindow(this);
    }

    public virtual void Close()
    {
        isOpen = false;
        if(animator)
            animator.Play("hide");
        UIManager.Instance.RemoveWindow(this);
    }
}