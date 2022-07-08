using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private List<IHurtboxResponder> _responders = new List<IHurtboxResponder>();
    [SerializeField] private PhysicalMaterial physicalMaterial;

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _hitColor;
    [SerializeField] private float _colorFadeStartDuration = .1f;
    [SerializeField] private float _colorFadeEndDuration = .5f;
    private Material _material;
    
    public void Start()
    {
        _material = _meshRenderer?.material;
        if(_material) _baseColor = _material.color;
    }

    // Called by weapon scripts after
    public bool Hit(StealthAgent agent, int softDamage, int hardDamage, DamageType damageType = DamageType.Blunt, Direction9 direction = Direction9.CENTER)
    {
        // Send hit data to all responders and see if atleast 1 returns true
        bool hit = (_responders.Count > 0)? false : true;
        foreach(IHurtboxResponder r in _responders)
            if(r.GetHit(agent, softDamage, hardDamage, damageType, direction))
                hit = true;

        if(hit)
        {
            // Hit Effects
            if(physicalMaterial)
                physicalMaterial.CollideEffect(transform.position, hardDamage);
        
            // Change hurtbox's material color if hit for testing
            if(_debug && _material)
                StartCoroutine(ColorChange(_colorFadeStartDuration, _colorFadeEndDuration));
        }
        else
        {
            // Deflect Effects
            if(physicalMaterial)
                physicalMaterial.CollideEffect(transform.position, hardDamage, MaterialType.Metal);
        }

        return hit;
    }

    public void AddResponder(IHurtboxResponder responder)
    {
        if(_responders.Contains(responder) == false)
            _responders.Add(responder);
    }

    IEnumerator ColorChange(float startFadeTime, float endFadeTime)
    {
        float timePassed = 0;
        float totalTime = startFadeTime + endFadeTime;
        float halfTime = totalTime / 2;
        while(timePassed < totalTime)
        {
            if(timePassed < startFadeTime)
                _material.color = Color.Lerp(_baseColor, _hitColor, timePassed / startFadeTime);
            else
                _material.color = Color.Lerp(_hitColor, _baseColor, (timePassed - startFadeTime) / endFadeTime);
                
            timePassed += Time.deltaTime;
            yield return null;
        }

        _material.color = _baseColor;
    }
}
