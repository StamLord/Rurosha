using UnityEngine;
using System;

public class SmartEnum<TEnum> where TEnum : Enum
{
    public TEnum Enum;

    public SmartEnum(TEnum Enum)
    {
        this.Enum = Enum;
    }
    
    public bool Equals(string name)
    {
        return Enum.ToString() == name;
    }

    public bool Equals(SmartEnum<TEnum> smartEnum)
    {
        var aType = Enum.GetType();
        var bType = smartEnum.Enum.GetType();
        
        if(aType != bType)
            return false;
        
        if((int)(object)(Enum) == (int)(object)(smartEnum.Enum))
            Debug.Log("Same Under Type");
        else
            Debug.Log("Diff Under Type");

        return (int)(object)(Enum) == (int)(object)(smartEnum.Enum);
    }

    public bool Equals<T>(SmartEnum<T> smartEnum) where T : Enum
    {
        var aType = Enum.GetType();
        var bType = smartEnum.GetType();
        
        if(aType != bType)
            return false;
        
        if((int)(object)(Enum) == (int)(object)(smartEnum))
            Debug.Log("Same Under Type");
        else
            Debug.Log("Diff Under Type");

        return (int)(object)(Enum) == (int)(object)(smartEnum);
    }

    public static bool operator == (SmartEnum<TEnum> smartEnum, string name)
    {
        return smartEnum.Equals(name);
    }

    public static bool operator != (SmartEnum<TEnum> smartEnum, string name)
    {
        return !smartEnum.Equals(name);
    }

    public static bool operator == (SmartEnum<TEnum> A, SmartEnum<TEnum> B)
    {
        return A.Equals(B);
    }

    public static bool operator != (SmartEnum<TEnum> A, SmartEnum<TEnum> B)
    {
        return !A.Equals(B);
    }

    public override string ToString()
    {
        return Enum.ToString();
    }
}
