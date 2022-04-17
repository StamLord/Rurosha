using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSoundAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private bool isActive;
    [SerializeField] private Transform playerTransform;
	[SerializeField] private Terrain t;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private AudioSource source;

    [Space(20)]

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] dirtStep;
    [SerializeField] private AudioClip[] grassStep;
    [SerializeField] private AudioClip[] leavesStep;

    [Space(20)]

    [Header("Step Interval")]
    [SerializeField] private float stepCheckEvery = .8f;
    [SerializeField] private float stepCheckEveryMax = .2f;
    [SerializeField] private float lastCheck;

    [Space(20)]

    [Header("Default Sound")]
    [SerializeField]private string defaultTexture = "Grass";

    public void SetActive(bool active)
    {
        isActive = active;
    }

    private void Update()
    {
        float stepInterval = Mathf.Lerp(stepCheckEvery, stepCheckEveryMax, rigidbody.velocity.magnitude / 15f);
        if(Time.time - lastCheck > stepInterval && rigidbody.velocity.magnitude > 1f && isActive)
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

    private string GetHighestTextureOnTerrain()
    {
        if(t == null) return defaultTexture;

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
