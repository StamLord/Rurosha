using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSoundAgent : MonoBehaviour
{
    public Transform playerTransform;
	public Terrain t;

    public AudioSource source;

    public AudioClip[] dirtStep;
    public AudioClip[] grassStep;
    public AudioClip[] leavesStep;

    public Rigidbody rigidbody;
    public GroundedState groundedState;
    public float stepCheckEvery = .8f;
    public float stepCheckEveryMax = .2f;
    float lastCheck;

    void Update()
    {
        float stepInterval = Mathf.Lerp(stepCheckEvery, stepCheckEveryMax, rigidbody.velocity.magnitude / 15f);
        if(Time.time - lastCheck > stepInterval && rigidbody.velocity.magnitude > 1f && groundedState.IsActive)
        {
            string tex = GetHighestTextureOnTerrain();
            switch(tex)
            {
                case "Dirt":
                    // Debug.Log("dirt");
                    source.PlayOneShot(dirtStep[Random.Range(0,dirtStep.Length)], .5f);
                break;
                case "Grass":
                    // Debug.Log("grass");
                    source.PlayOneShot(grassStep[Random.Range(0,grassStep.Length)], .5f);
                break;
                case "Leaves":
                    // Debug.Log("leaves");
                    source.PlayOneShot(leavesStep[Random.Range(0,leavesStep.Length)], .5f);
                break;
            }

            lastCheck = Time.time;

            //TerrainChecker.SampleDetailLayer(TerrainChecker.ConvertPosition(playerTransform.position, t), t, 2);
        }
    }

    string GetHighestTextureOnTerrain()
    {
        float[] values = TerrainChecker.SampleTextures(TerrainChecker.ConvertPosition(playerTransform.position, t), t);
        
        float highestValue = -1f;
        int highestTexture = -1;

        for (int i = 0; i < values.Length; i++)
        {
            if(values[i] >= highestValue)
            { 
                highestValue = values[i];
                highestTexture = i;
            }
        }

        return t.terrainData.terrainLayers[highestTexture].name;
    }
}
