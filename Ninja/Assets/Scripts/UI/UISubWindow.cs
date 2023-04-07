using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubWindow : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] protected Animator animator;

    [Header("Container")]
    [SerializeField] private GameObject container;

    private bool isOpen;
    public bool IsOpen {get {return isOpen;}}

    public virtual void ProcessInput(Vector3 axis, bool select){}
    public virtual bool Select(int index){return false;}
    
    public virtual void Open()
    {
        isOpen = true;
        if(animator)
            animator.Play("show");
        else if(container)
            container.SetActive(true);
    }

    public virtual void Close()
    {
        isOpen = false;
        if(animator)
            animator.Play("hide");
        else if(container)
            container.SetActive(false);
    }
}
