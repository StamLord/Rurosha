using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KatanaDirectionIndicator : MonoBehaviour
{
    [SerializeField] private Image[] indicators;
    [SerializeField] private KatanaDirectional katana;

    private void Update()
    {
        Direction9 direction = katana.GetDirection();

        for (var i = 0; i < indicators.Length; i++)
        {
            Color c = indicators[i].color;
            c.a = (katana.gameObject.activeSelf)? (i == (int)direction)? 1 : 0 : 0;
            indicators[i].color = c;
        }
    }
}
