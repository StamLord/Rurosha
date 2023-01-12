using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralTree : MonoBehaviour
{
    [Header("Random Seed")]
    [SerializeField] private int seed;

    [Header("Bark")]
    [SerializeField] private GameObject bark;
    [SerializeField] private Vector2 heightVariability = new Vector2(10,15);
    private float height;

    [Header("Branch")]
    [SerializeField] private GameObject branch;
    [SerializeField] private Vector2 branchSpread = new Vector2(.2f, .8f);
    [SerializeField] private float branchVerticalSpace = 2f;
    [SerializeField] private AnimationCurve branchAmount;
    [SerializeField] private AnimationCurve branchLength;
    [SerializeField] private float levelRotationoffset = 45;
    [SerializeField] private float branchLengthRandom = .25f;
    [SerializeField] private Vector2 xRotation = new Vector2(0,0);

    [SerializeField] private bool update;

    [SerializeField] private List<GameObject> instantiatedBranches = new List<GameObject>();
    
    private void OnValidate() 
    {
        StartCoroutine("GenerateTree");
    }
    
    private IEnumerator GenerateTree()
    {
        Random.InitState(seed);

        yield return StartCoroutine("ClearBranchesCoroutine");
        UpdateBark();
        UpdateBranches();
    }

    private void UpdateBark()
    {
        Vector3 scale = bark.transform.localScale;
        height = Random.Range(heightVariability.x, heightVariability.y);
        scale.y = height;

        bark.transform.localScale = scale;
    }

    private void UpdateBranches()
    {
        // Start / End height of branches
        float startHeight = height * branchSpread.x;
        float endHeight = height * branchSpread.y;
        float validHeight = endHeight - startHeight;

        // Amount of levels of branches
        int levels = Mathf.FloorToInt(validHeight / branchVerticalSpace);

        for (int i = 0; i < levels; i++)
        {
            float curHeight = startHeight + i * branchVerticalSpace;
            
            // Get amount of branches on this level
            int branches = Mathf.FloorToInt(branchAmount.Evaluate((curHeight - startHeight) / validHeight));

            // Rotation of level
            float rotationOffest = levelRotationoffset * i;

            for (int j = 0; j < branches; j++)
            {
                // Calculate position
                Vector3 position = transform.position;
                position.y += curHeight;

                // Calculate rotation
                float xRot = Random.Range(xRotation.x, xRotation.y);
                Quaternion rotation = Quaternion.Euler(xRot, rotationOffest + 360 / branches * j, 0);

                GameObject b = Instantiate(branch, position, rotation, transform);

                // Scale length
                Vector3 scale = b.transform.localScale;
                scale.z = branchLength.Evaluate((curHeight - startHeight) / validHeight);
                scale.z *= Random.Range(1, 1 + branchLengthRandom);

                b.transform.localScale = scale;

                instantiatedBranches.Add(b);
            }
        }
    }

    private void ClearBranches()
    {
        StartCoroutine("ClearBranchesCoroutine");
    }

    private IEnumerator ClearBranchesCoroutine()
    {
        yield return null;
        foreach(GameObject b in instantiatedBranches)
        {
            if(Application.isEditor)
                DestroyImmediate(b);
            else
                Destroy(b);
        }

        instantiatedBranches.Clear();
    }


}
