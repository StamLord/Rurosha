using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueChoiceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _choiceText;
    [SerializeField] private int _index;
    [SerializeField] private bool _askNode;
    [SerializeField] private Button _button;

    public void SetupChoice(string text, int index, bool askNode = false)
    {
        _choiceText.text = (index + 1)  + ". " + text;
        _index = index;
        _askNode = askNode;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(SelectChoice);
    }

    //Added as a Listener to Button component
    public void SelectChoice()
    {
        if(_askNode)
            DialogueManager.Instance.SelectAsk();
        else
            DialogueManager.Instance.SelectChoice(_index);
    }
}
