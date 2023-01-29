using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDecalPool : MonoBehaviour
{
    [SerializeField] private int maxDecals = 100;
    [SerializeField] private GameObject decal;
    [SerializeField] private Vector2 sizeMinMax = new Vector2(1f,2f);
    
    private ParticleDecalData[] particleData;
    private List<GameObject> decals = new List<GameObject>();
    private int particleDecalDataIndex;

    private void Start()
    {
        particleData = new ParticleDecalData[maxDecals];
        
        for (int i = 0; i < maxDecals; i++)
            particleData[i] = new ParticleDecalData();
    }

    public void ParticleHit(ParticleCollisionEvent particleCollisionEvent)
    {
        SetParticleData(particleCollisionEvent);
        UpdateParticles();
    }

    private void SetParticleData(ParticleCollisionEvent collisionEvent)
    {
        if(particleDecalDataIndex >= maxDecals)
            particleDecalDataIndex = 0;
        
        particleData[particleDecalDataIndex].parent = collisionEvent.colliderComponent.transform;
        particleData[particleDecalDataIndex].position = collisionEvent.intersection;
        particleData[particleDecalDataIndex].normal = collisionEvent.normal;
        particleData[particleDecalDataIndex].rotation = Random.Range(0f,360f);
        particleData[particleDecalDataIndex].size = Random.Range(sizeMinMax.x,sizeMinMax.y);
        particleData[particleDecalDataIndex].color = Color.red;

        particleDecalDataIndex++;
    }

    private void UpdateParticles()
    {
        for (var i = 0; i < particleData.Length; i++)
        {
            if(i >= decals.Count)
            {
                GameObject obj = Instantiate(decal, particleData[i].position, Quaternion.identity, particleData[i].parent);
                obj.transform.forward = -particleData[i].normal;
                obj.transform.rotation = Quaternion.Euler(
                    obj.transform.rotation.eulerAngles.x, 
                    obj.transform.rotation.eulerAngles.y, 
                    particleData[i].rotation);
                obj.transform.localScale = Vector3.one * particleData[i].size;
                decals.Add(obj);
            }
            else
            {
                decals[i].transform.position = particleData[i].position;
                decals[i].transform.forward = -particleData[i].normal;
                decals[i].transform.rotation = Quaternion.Euler(
                    decals[i].transform.rotation.eulerAngles.x, 
                    decals[i].transform.rotation.eulerAngles.y, 
                    particleData[i].rotation);
                decals[i].transform.localScale = Vector3.one * particleData[i].size;
                decals[i].transform.parent = particleData[i].parent;
            }

        }
    }
}
