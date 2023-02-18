using UnityEngine;
using TMPro;

public class UIItemPrice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Vector3 parentOffset;
    private Color originalColor;
    private Camera cam;

    private void Start() 
    {
        parentOffset = transform.localPosition;
        originalColor = text.color;
        cam = Camera.main;
    }

    private void Update() 
    {
        // Face camera
        transform.forward = transform.position - cam.transform.position;
        
        // Keep original offset
        transform.position = transform.parent.position + parentOffset;

        // Visible based on camera proximity
        SetVisible(Vector3.Distance(cam.transform.position, transform.position) < 2f);
    }

    public void UpdatePrice(int amount)
    {
        text.text = "" + amount;
    }

    public void SetVisible(bool visible)
    {
        text.color = (visible)? originalColor : new Color(1,1,1,0);
    }
}
