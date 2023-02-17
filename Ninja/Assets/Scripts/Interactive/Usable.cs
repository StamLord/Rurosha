using UnityEngine;

public class Usable : MonoBehaviour
{
    public Outline outline;
    [SerializeField] protected string interactionText = "Use";
    
    void Awake()
    {
        InitializeOutline();
    }

    public void InitializeOutline()
    {
        if(outline == null) 
            outline = GetComponent<Outline>();
        Highlight(false);
    }

    public void Highlight(bool on)
    {
        if(outline) outline.enabled = on;
    }

    public virtual void Use(Interactor interactor)
    {
        
    }

    public virtual string GetText(Interactor interactor = null)
    {
        return interactionText;
    }
}
