using UnityEngine;

public class SenseMoneyVFX : MonoBehaviour
{
    public static bool Active;

    [SerializeField] private ParticleSystem vfx;

    private void Update() 
    {
            if(vfx.enableEmission != Active)
                vfx.enableEmission = Active;
    }
}
