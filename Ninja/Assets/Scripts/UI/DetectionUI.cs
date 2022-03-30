using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectionUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private StealthAgent stealthAgent;

    [Header("Detect Radar Prefab")]
    [SerializeField] private Image detectImage;
    [SerializeField] private List<Image> images;

    [Header("Detect Radar Settings")]
    [SerializeField] private float detectRadius = 1f;

    [SerializeField] private Vector3 imageScaleStart = Vector3.one;
    [SerializeField] private Vector3 imageScaleEnd = Vector3.one;

    [Header("Detect Eye")]
    [SerializeField] private bool displayEye;
    [SerializeField] private Image eyeOpen;
    [SerializeField] private Image eyeClosed;

    private void Update()
    {
        UpdateEye();
        UpdateRadar();    
    }

    private void UpdateRadar()
    {
        int index = 0;
        foreach(KeyValuePair<AwarenessAgent,float> pair in stealthAgent.Awareness)
        {
            // Instantiate image if not enough
            if(images.Count <= index)
                images.Add(CreateImage());

            // Turn the vector from player to AwarenessAgent (world space) to 2D vector (screen space)
            Vector3 targetDelta = pair.Key.transform.position - stealthAgent.transform.position + stealthAgent.transform.forward;
            targetDelta = stealthAgent.transform.InverseTransformVector(targetDelta);
            Vector2 screenDelta = new Vector2(targetDelta.x, targetDelta.z).normalized;
            
            // Update image position and rotationon screen
            images[index].rectTransform.localPosition = screenDelta * detectRadius;
            float angle = Mathf.Atan2(screenDelta.y, screenDelta.x) * Mathf.Rad2Deg;
            images[index].rectTransform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            // Update image color and scale
            images[index].color = new Color(images[index].color.r, images[index].color.g, images[index].color.b, pair.Value);
            images[index].rectTransform.localScale = Vector3.Lerp(imageScaleStart, imageScaleEnd, pair.Value);

            index++;
        }

        // Turn off all images left
        for (var i = index; i < images.Count; i++)
        {
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f);
        }
    }

    private Image CreateImage()
    {
        Image image = Instantiate(detectImage, transform);
        return image;
    }

    private void UpdateEye()
    {
        if(displayEye == false || stealthAgent == null) return;
        
        float value = stealthAgent.DetectedValue;

        Color openColorVisible = eyeOpen.color;
        openColorVisible.a = 1f;
        Color openColorHidden= eyeOpen.color;
        openColorHidden.a = 0f;

        Color closedColorVisible = eyeClosed.color;
        closedColorVisible.a = 1f;
        Color closedColorHidden= eyeClosed.color;
        closedColorHidden.a = 0f;

        if(value > 0)
        {
            eyeOpen.color = Color.Lerp(openColorHidden, openColorVisible, value);
            eyeClosed.color = closedColorHidden;
        }
        else
        {
            eyeOpen.color = openColorHidden;
            eyeClosed.color = closedColorVisible;
        }
    }
}
