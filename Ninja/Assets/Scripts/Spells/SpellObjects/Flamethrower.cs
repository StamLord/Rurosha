using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : SpellObject, IHitboxResponder
{
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private ParticleSystem vfx;
    
    [SerializeField] private float duration = 3;
    [SerializeField] private int softDPS;
    [SerializeField] private int hardDPS;
    
    private SpellManager manager;
    private float startTime;

    private void Start()
    {
        hitbox.SetResponder(this);
    }

    public override void Activate(SpellManager manager)
    {
        this.manager = manager;
        vfx.Play();

        StartCoroutine("Active");
    }

    private IEnumerator Active()
    {
        startTime = Time.time;

        while(Time.time - startTime < duration)
        {
            hitbox.StartColliding(true);
            yield return null;
        }

        Stop();
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        // Ignore caster
        if(collider.transform.root == manager.transform.root)
            return;
        
        Hurtbox hurt = collider.GetComponent<Hurtbox>();
        if(hurt)
            hurt.Hit(Mathf.CeilToInt(softDPS * Time.deltaTime), Mathf.CeilToInt(hardDPS * Time.deltaTime), DamageType.Blunt);
    }

    public override void Stop()
    {
        vfx.Stop();
    }

    public void UpdateColliderState(bool state)
    {

    }
}
