using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubEmitterCorrector : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private float updatesPerSecond;
    [SerializeField] private bool debug;

    private ParticleSystem.Particle[] particles;
    private float updateInterval;
    private float lastUpdate;

    private void OnValidate() 
    {
        updateInterval = 1 / updatesPerSecond;    
    }

    private void Update()
    {
        if(Time.time - lastUpdate > updateInterval)
        {
            UpdateParticles();
            lastUpdate = Time.time;
        }
    }

    private void UpdateParticles()
    {
        if(particleSystem == null) return;

        if(particles == null || particles.Length < particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        
        int aliveParticles = particleSystem.GetParticles(particles);

        for (int i = 0; i < aliveParticles; i++)
        {   
            RaycastHit hit;
            // Raycast from .1f units above paticle downwards
            if(Physics.Raycast(particles[i].position + Vector3.up * .1f, Vector3.down, out hit))
            {
                Quaternion rotation =  Quaternion.LookRotation(-hit.normal);
                particles[i].rotation3D = rotation.eulerAngles;
                //particles[i].position = hit.point;
                
                if(debug)
                    Debug.DrawRay(particles[i].position, hit.normal, Color.red);
            }
        }

        particleSystem.SetParticles(particles, aliveParticles);
    }
}
