using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProj : MonoBehaviour, IHitboxResponder
{
    [Header("Bomb Settings")]
    [SerializeField] private float countdown = 3;
    [SerializeField] private float radius = 5;
    [SerializeField] private DamageType damageType = DamageType.Pierce;
    [SerializeField] private int softDamage = 10;
    [SerializeField] private int hardDamage = 20;
    
    [Header("References")]
    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Hitbox hitbox;

    private bool started;
    private float startTime;
    private List<GameObject> objectsCollided = new List<GameObject>();

    void Start()
    {
        hitbox.SetResponder(this);
        StartTimer();
    }

    public void StartTimer()
    {
        started = true;
        startTime = Time.time;
    }

    void Update()
    {
        if(started == false)
            return;
        
        if(Time.time >= startTime + countdown)
            Explode();
    }
    
    private void Explode()
    {
        Debug.Log("BoOM");
        StopTimer();
        if(visual) visual.SetActive(false);
        if(explosion) explosion.SetActive(true);
        hitbox.StartColliding(true);
    }

    private void StopTimer()
    {
        started = false;
    }

    public void CollisionWith(Collider col)
    {
        Hurtbox hurtbox = col.GetComponent<Hurtbox>();
        if(hurtbox)
        { 
            Debug.Log("Collide with " + col.gameObject.name);
            // Avoid triggering multiple hurtboxes with the same parent GameObject
            if(objectsCollided.Contains(hurtbox.transform.parent.gameObject) == false)
            {
                hurtbox.Hit(softDamage, hardDamage, damageType);
                objectsCollided.Add(hurtbox.transform.parent.gameObject);
            } 
        }
    }

    public void UpdateColliderState(bool state)
    {
        if(state == false)
            objectsCollided.Clear();
    }
}
