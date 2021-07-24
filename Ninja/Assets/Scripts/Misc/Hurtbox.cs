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
            StartCoroutine(ColorChange(.5f));
    }

    public void AddResponder(IHurtboxResponder responder)
    {
        if(_responders.Contains(responder) == false)
            _responders.Add(responder);
    }

    IEnumerator ColorChange(float time)
    {
        float timePassed = 0;

        while(timePassed < time)
        {
            if(timePassed < time / 2 )
                _material.color = Color.Lerp(_baseColor, _hitColor, timePassed / time / 2);
            else
                _material.color = Color.Lerp(_hitColor, _baseColor, timePassed - time / 2 / time / 2);
                
            timePassed += Time.deltaTime;
            yield return null;
        }

        _material.color = _baseColor;
    }
}
