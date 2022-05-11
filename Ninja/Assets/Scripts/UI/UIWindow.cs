using UnityEngine;

public class UIWindow : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    public virtual void ProcessInput(Vector3 axis, bool select){}
    public virtual bool Select(int index){return false;}
    
    public virtual void Open()
    {
        if(animator)
            animator.Play("show");
        UIManager.Instance.AddWindow(this);
    }

    public virtual void Close()
    {
        if(animator)
            animator.Play("hide");
        UIManager.Instance.RemoveWindow(this);
    }
}