using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChan
    {
    public class SprintColliderInitializer : MonoBehaviour
    {
        private void OnValidate() 
        {
            SpringCollider[] colliders = GetComponentsInChildren<SpringCollider>();
            SpringBone[] bones = GetComponentsInChildren<SpringBone>();

            foreach(SpringBone b in bones)
                b.colliders = colliders;
        }
    }
}