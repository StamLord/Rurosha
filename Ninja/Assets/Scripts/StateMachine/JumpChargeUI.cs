using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpChargeUI : MonoBehaviour
{
    [SerializeField] private JumpState _jumpState;
    [SerializeField] private GameObject _parent;
    [SerializeField] private Image _fillBar;
    [SerializeField] private float _appearAtPercentage = .1f;

    void Start()
    {
        if(_jumpState)
            _jumpState.OnJumpCharge += UpdateJumpChargeBar;
    }

    void UpdateJumpChargeBar(float percentage)
    {
        if(percentage < 0)
            _parent.SetActive(false);
        else if (percentage >= _appearAtPercentage)
            _parent.SetActive(true);
            _fillBar.fillAmount = percentage;
    }
}
