using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform _follow;

    void Update()
    {
        transform.position = _follow.position;
    }
}
