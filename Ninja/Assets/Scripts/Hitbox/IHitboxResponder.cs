using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxResponder
{
     void CollisionWith(Collider collider, Hitbox hitbox);

     void UpdateColliderState(bool newState);
}
