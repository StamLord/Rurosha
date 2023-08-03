using System.Collections;
using UnityEngine;

public class RagdollBreaker : MonoBehaviour
{
    [SerializeField] private CharacterJoint[] joints;
    [SerializeField][Range(0,1)] private float breakChance = .5f;
    [SerializeField] private float delay = 1f;

    private void OnEnable() 
    {
        StartCoroutine("Delay");
    }
    
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        BreakJoints();
    }

    private void BreakJoints()
    {
        foreach(CharacterJoint j in joints)
        {
            float r = Random.Range(0, 1f);
            if(r <= breakChance)
                j.breakForce = 1f;
        }
    }
}
