using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellObject : MonoBehaviour
{
    public Vector3 offset;
    public abstract void Activate(SpellManager manager);
    public virtual void Stop(){}
}