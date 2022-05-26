using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColliderManager : MonoBehaviour
{
    public enum BodyCollider {STAND, CROUCH, CLIMB}

    [SerializeField] private Collider[] body;
    [SerializeField] private Collider[] legs;

    [SerializeField] private int bodyActive;
    [SerializeField] private int legsActive;

    public void SetBody(BodyCollider type)
    {
        SetBody((int)type);
    }

    public void SetLegs(BodyCollider type)
    {
        SetLegs((int)type);
    }

    private void SetBody(int index)
    {
        if(index >= body.Length)
            return;
        
        body[bodyActive].enabled = false;
        body[index].enabled = true;
        bodyActive = index;
    }

    private void SetLegs(int index)
    {
        if(index >= legs.Length)
            return;
        
        legs[legsActive].enabled = false;
        legs[index].enabled = true;
        legsActive = index;
    }
}
