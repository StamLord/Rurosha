using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : HeatConductor
{
    [SerializeField] private float _cookTemperature;
    [SerializeField] private float _cooked;
    [SerializeField] private float _cookingSpeed = .005f;

    public float Cooked {get{return _cooked;}}

    [SerializeField] private float _burned;
    [SerializeField] private float _burnSpeed = .0025f;

    public float Burned {get{return _burned;}}

    public MeshRenderer meshRenderer;
    public Color baseColor;
    public Color cookedColor;
    public Color burnedColor;

    void Start() 
    {
        baseColor = meshRenderer.material.color;
    }

    void Update()
    {
        Diffuse();
        
        if(Temperature >= _cookTemperature)
        {
            if(_cooked < 1f) 
                Cook();
            else 
                Burn();
        }

        meshRenderer.material.color = Color.Lerp(baseColor, cookedColor, _cooked);
        meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, burnedColor, _burned);
    }

    void Cook()
    {
        _cooked = Mathf.Clamp01(_cooked + (Temperature - _cookTemperature) * _cookingSpeed * Time.deltaTime);
    }

    void Burn()
    {
        _burned = Mathf.Clamp01(_burned + (Temperature - _cookTemperature) * _cookingSpeed * Time.deltaTime);
    }
}
