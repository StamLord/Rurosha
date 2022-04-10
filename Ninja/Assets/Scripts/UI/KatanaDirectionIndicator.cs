using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KatanaDirectionIndicator : MonoBehaviour
{
    [SerializeField] private Image[] indicators;
    [SerializeField] private Image[] hits;
    [SerializeField] private KatanaDirectional katana;

    private void Update()
    {
        // Direction indicator
        Direction9 direction = katana.GetDirection();

        for (var i = 0; i < indicators.Length; i++)
        {
            Color c = indicators[i].color;
            c.a = (katana.gameObject.activeSelf)? (i == (int)direction)? 1 : 0 : 0;
            indicators[i].color = c;
        }

        // Combo hits
        for (var i = 0; i < hits.Length; i++)
        {
            Color c = hits[i].color;
            bool inCombo = katana.combo.Contains((Direction9)i);
            c.a = (katana.gameObject.activeSelf)? (inCombo)? 1 : 0 : 0;
            hits[i].color = c;
        }
    }
}
