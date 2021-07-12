using System;
using UnityEngine;
using TMPro;

public class DialogueActionButton : MonoBehaviour
{
    public enum DialogueActionType {CHARM, JOKE, THREAT, BRIBE}
    [SerializeField] private DialogueActionType _actionType;

    [SerializeField] private GameObject _subWindow;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private int _value;

    public void OpenSubWindow()
    {
        _subWindow.SetActive(!_subWindow.activeSelf);
    }

    public void ChangeValue(int newValue)
    {
        _value = newValue;
    }

    public void PerformAction()
    {
        switch(_actionType)
        {
            case DialogueActionType.CHARM:
                DialogueManager.Instance.Charm();
                break;
            case DialogueActionType.JOKE:
                DialogueManager.Instance.Joke();
                break;
            case DialogueActionType.THREAT:
                DialogueManager.Instance.Threat();
                break;
            case DialogueActionType.BRIBE:
                int money = (_inputField.text.Length > 0) ? Int32.Parse(_inputField.text) : 0;
                DialogueManager.Instance.Bribe(money);
                break;
        }
        
    }
}
