using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeDependant<T>
{
    private Attribute attribute;
    [SerializeField] private string attributeName;
    [SerializeField] private T[] values;

    public Attribute Attribute {get {return attribute;}}
    public string AttributeName {get {return attributeName;}}

    public AttributeDependant(Attribute attribute, T[] values)
    {
        this.attribute = attribute;
        this.values = values;
    }

    public AttributeDependant(string attributeName, T[] values)
    {
        this.attributeName = attributeName;
        this.values = values;
    }

    private T GetValue()
    {
        if(attribute == null)
            return default(T);
        return values[attribute.Level - 1];
    }

    public T GetValue(CharacterStats stats)
    {   
        if(attribute == null)
            attribute = stats.FindAttribute(attributeName);
        
        return GetValue();
    }

    public T GetValue(Attribute attribute)
    {   
        return values[attribute.Level - 1];
    }

    public T GetValueAt(int index)
    {
        return values[index];
    }
}
