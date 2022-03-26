using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxResponder
{
     void CollisionWith(Collider collider);

     void UpdateColliderState(bool newState);
}
