using UnityEngine;

public class Lantern : TimeBased
{
    [SerializeField] private new Light light;
    [SerializeField] private MeshRenderer meshRenderer;
    
    protected override void StartAction()
    {
        base.StartAction();
        light.enabled = true;
        meshRenderer?.material.EnableKeyword("_EMISSION");
    }

    protected override void EndAction()
    {
        base.EndAction();
        light.enabled = false;
        meshRenderer?.material.DisableKeyword("_EMISSION");
    }
}
