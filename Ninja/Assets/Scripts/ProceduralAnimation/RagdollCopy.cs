using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCopy : MonoBehaviour
{
    [SerializeField] private Transform reference;
    
    private void OnEnable() 
    {
        CopyBone(transform, reference);
    }

    private void CopyBone(Transform target, Transform reference)
    {
        //Debug.Log(target + " <= " + reference);
        target.localPosition = reference.localPosition;
        target.localRotation = reference.localRotation;

        int lowestBoneNum = Mathf.Min(target.childCount, reference.childCount);

        for(int i = 0; i < lowestBoneNum; i++)
        {
            Transform t = target.GetChild(i);
            Transform r = reference.GetChild(i);
            if(t != null && r != null)            
                CopyBone(t, r);
        }
    }
}
