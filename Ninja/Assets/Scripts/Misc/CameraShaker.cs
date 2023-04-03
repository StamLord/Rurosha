using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] private new Camera camera;

    private enum ShakeType {Random, Smart};
    [SerializeField] private ShakeType shakeType;

    // Following properties are used to debug in the custom editor
    [SerializeField] private float strength = 10; 
    [SerializeField] private float duration = 1; 
    [SerializeField] private float frequency = 5; // 5 a second

    private Vector3 lastSmartShake;
    private bool firstSmartShakeDone;

    public void StartShake(float strength, float duration, float frequency)
    {
       StartCoroutine(Shake(strength, duration, frequency));
    }

    private IEnumerator Shake(float strength, float duration, float frequency)
    {
        float startTime = Time.time;
        float p = 0;
        float freq = 1 / (frequency * duration);
        float nextShakeP = 0;
        float lastShakeTime = Time.time;
        Vector3 origin = camera.transform.localPosition;
        Vector3 lastCamPos = origin;
        Vector3 currentShake = Vector3.zero;

        // Reset flag for smart shake
        if(shakeType == ShakeType.Smart)
            firstSmartShakeDone = false;
        
        // Shake loop
        while(p <= 1)
        {
            p = (Time.time - startTime) / duration;

            // Time for next shake
            if(p > nextShakeP)
            {
                float str = strength * (1 - p); // Weaker as we get closer to end
                Vector3 pos = Vector3.zero;

                switch(shakeType)
                {
                    case ShakeType.Random:
                        pos = RandomShake(str);
                        break;
                    case ShakeType.Smart:
                        pos = SmartShake(str);
                        break;
                }

                lastCamPos = camera.transform.localPosition;
                currentShake = origin + camera.transform.InverseTransformVector(pos);
                lastShakeTime = Time.time;
                nextShakeP += freq;
            }

            // Lerp to shake position
            else
            {
                float t = (Time.time - lastShakeTime) / freq;
                camera.transform.localPosition = Vector3.Lerp(
                    lastCamPos, 
                    currentShake, 
                    t);
            }
            
            yield return null;
        }

        // Return to origin
        float returnStart = Time.time;
        float j = 0;
        lastCamPos = camera.transform.localPosition;

        while(j <= 1)
        {
            j = (Time.time - returnStart) / freq;
            camera.transform.localPosition = Vector3.Lerp(
                lastCamPos, 
                origin, 
                j);
            
            yield return null;
        }
    }

    private Vector3 RandomShake(float strength)
    {
        return new Vector3(
            Random.Range(-strength, strength),
            Random.Range(-strength, strength),
            0); 
    }

    private Vector3 SmartShake(float strength)
    {
        Vector3 vector = Vector3.zero;

        // First is complete random
        if(firstSmartShakeDone == false)
        {
            vector.x = Random.Range(-strength, strength);
            vector.y = Random.Range(-strength, strength);
            firstSmartShakeDone = true;
        }
        else
        {
            // Go in opposite direction to last shake
            vector = lastSmartShake * -1;

            // Add variation
            vector.x += Random.Range(-strength * .25f, strength * .25f);
            vector.y += Random.Range(-strength * .25f, strength * .25f);
        }
        
        vector = vector.normalized * strength;
        lastSmartShake = vector;

        return vector;
    }
}
