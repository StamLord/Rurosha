using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KappaChakraVisualizer : MonoBehaviour
{
    [SerializeField] private ChakraManager manager;
    [SerializeField] private ChakraType chakra = ChakraType.WATER;
    [SerializeField] private Transform target;
    [SerializeField] private float emptyHeight;
    [SerializeField] private float fullHeight;

    private void LateUpdate()
    {
        Vector3 pos = target.localPosition;
        
        pos.y = Mathf.Lerp(emptyHeight, fullHeight, manager.GetChakraAmount(chakra));
        target.localPosition = pos;
    }
}
