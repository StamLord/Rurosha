using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLauncher : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private ParticleSystem splatterParticles;
    [SerializeField] private ParticleDecalPool decalPool;

    List<ParticleCollisionEvent> particleCollisionEvents = new List<ParticleCollisionEvent>();

    private IEnumerator OnParticleCollision(GameObject other) 
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleSystem, other, particleCollisionEvents);

        for (int i = 0; i < particleCollisionEvents.Count; i++)
        {
            if(decalPool)
                decalPool.ParticleHit(particleCollisionEvents[i]);
            if(splatterParticles)
                EmitAtLocation(particleCollisionEvents[i]);
            yield return null;
        }
    }

    private void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
    {
        splatterParticles.transform.position = particleCollisionEvent.intersection;
        splatterParticles.transform.up = particleCollisionEvent.normal;
        splatterParticles.Emit(1);
    }
}
