using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private List<IHurtboxResponder> _responders = new List<IHurtboxResponder>();
    [SerializeField] private Material material;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color hitColor;
    
    public void Start()
    {
        material = GetComponentInChildren<MeshRenderer>().material;
        baseColor = material.color;
    }

    public void GetHit(int damage)
    {
        GetHit(damage, DamageType.Blunt);
    }

    public void GetHit(int damage, DamageType damageType)
    {
        foreach(IHurtboxResponder r in _responders)
            r.GetHit(damage, damageType);

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
                material.color = Color.Lerp(baseColor, hitColor, timePassed / time / 2);
            else
                material.color = Color.Lerp(hitColor, baseColor, timePassed - time / 2 / time / 2);
                
            timePassed += Time.deltaTime;
            yield return null;
        }

        material.color = baseColor;
        
    }
}
