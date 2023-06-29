using UnityEngine;

public class VfxComponent : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;

    public void Play()
    {
        particleSystem.Play();
    }

    public void Pause()
    {
        particleSystem.Pause();
    }

    public void Stop()
    {
        particleSystem.Stop();
    }
}
