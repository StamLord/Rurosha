using System.Collections.Generic;
using UnityEngine;

public class RagdollCopy : MonoBehaviour
{
    [SerializeField] private Transform reference;
    [SerializeField] private List<string> ignoreTransformNames = new List<string>();
    [SerializeField] private bool debug;

    private void OnEnable() 
    {
        CopyBone(transform, reference);
    }

    private void CopyBone(Transform target, Transform reference)
    {
        if(ignoreTransformNames.Contains(reference.name) || ignoreTransformNames.Contains(target.name))
        {
            if(debug)
                Debug.Log("Ignoring transforms: " + target + ", " + reference);
            
            return;
        }
        
        if(debug)
            Debug.Log(target + " <= " + reference);
        
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
