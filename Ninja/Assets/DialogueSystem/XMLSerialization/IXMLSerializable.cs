using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IXMLSerializable
{
    public void Serialize(string fileName);

    public void Deserialize(string fileName);
}