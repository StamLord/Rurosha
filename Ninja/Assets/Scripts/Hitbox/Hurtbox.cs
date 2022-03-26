using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private List<IHurtboxResponder> _responders = new List<IHurtboxResponder>();

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private Material _material;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _hitColor;
    [SerializeField] private float _colorFadeStartDuration = .1f;
    [SerializeField] private float _colorFadeEndDuration = .5f;
    
    public void Start()
    {
        _material = GetComponentInChildren<MeshRenderer>()?.material;
        if(_material) _baseColor = _material.color;
    }

    // Called by weapon scripts after
    public void Hit(int softDamage, int hardtDamage)
    {
        Hit(softDamage, hardtDamage, DamageType.Blunt);
    }

    public void Hit(int softDamage, int hardDamage, DamageType damageType)
    {
        foreach(IHurtboxResponder r in _responders)
            r.GetHit(softDamage, hardDamage, damageType);

        if(_debug && _material)
            StartCoroutine(ColorChange(_colorFadeStartDuration, _colorFadeEndDuration));
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
