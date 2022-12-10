using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemHighlighter : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private Animator animator;
    
    [SerializeField] private string pointerEnter;
    [SerializeField] private string pointerExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.Play(pointerEnter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Prevent exiting when hovering on child objects
        if (!eventData.fullyExited)
            return;
        
        animator.Play(pointerExit);
    }
}
