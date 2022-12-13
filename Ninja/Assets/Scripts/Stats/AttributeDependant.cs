using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeDependant<T>
{
    private Attribute attribute;
    [SerializeField] private AttributeType attributeType;
    [SerializeField] private T[] values;

    public Attribute Attribute {get {return attribute;}}
    public AttributeType AttributeType {get {return attributeType;}}

    public AttributeDependant(Attribute attribute, T[] values)
    {
        this.attribute = attribute;
        this.values = values;
    }

    public AttributeDependant(AttributeType attributeType, T[] values)
    {
        this.attributeType = attributeType;
        this.values = values;
    }

    private T GetValue()
    {
        if(attribute == null || attribute.Modified <= 0)
            return values[0];
        if(attribute.Modified > values.Length - 1)
            return values[values.Length - 1];
        
        return values[attribute.Modified - 1];
    }

    public T GetValue(CharacterStats stats)
    {   
        if(attribute == null)
            attribute = stats.FindAttribute(attributeType);
        
        return GetValue();
    }

    public T GetValue(Attribute attribute)
    {   
        if(attribute.Modified > values.Length - 1)
            return values[values.Length - 1];
        return values[attribute.Modified - 1];
    }

    public T GetValueAt(int index)
    {
        return values[index];
    }
}
