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
    public SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Material _material;
    
    public Color baseColor;
    public Color cookedColor;
    public Color burnedColor;

    void Start() 
    {
        if(skinnedMeshRenderer)
            _material = skinnedMeshRenderer.material;
        else if (meshRenderer)
            _material = meshRenderer.material;

        if(_material) baseColor = _material.color;
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

        if(_material)
        {
            _material.color = Color.Lerp(baseColor, cookedColor, _cooked);
            _material.color = Color.Lerp(_material.color, burnedColor, _burned);
        }
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
