using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDialogueManager : UIWindow
{
    #region Singleton

    private static UIDialogueManager _instance;
    public static UIDialogueManager Instance { get{ return (_instance == null) ? _instance = FindUIManager() : _instance;}}

    private static UIDialogueManager FindUIManager()
    {
        UIDialogueManager um = FindObjectOfType<UIDialogueManager>();
        if(um == null) 
        {
            GameObject go = new GameObject("DialogueManager");
            um = go.AddComponent<UIDialogueManager>();
        }

        return um;
    }

    private UIDialogueManager SetupInstance()
    {
        if(_instance != null && _instance != this)
        {
            Debug.LogWarning("There is more than 1 DialogueManager. Destroying object!");
            Destroy(gameObject);
        }
        else
            _instance = this;
        
        return _instance;
    }

    #endregion

    #region UI

    [Header("General")]
    [SerializeField] private GameObject _dialogueWindow;
    [SerializeField] private TextMeshProUGUI _outputBox;
    [SerializeField] private Transform _choicesParent;
    [SerializeField] private GameObject _choicePrefab;
    [SerializeField] private List<DialogueChoiceUI> _choices = new List<DialogueChoiceUI>();
    [SerializeField] private float letterDrawTime = .02f;
    
    [Space(20)]
    [Header("Interactions")]
    [SerializeField] private List<Button> _interactionButtons = new List<Button>();
    [SerializeField] private GameObject[] likeBar;
    [SerializeField] private GameObject[] fearBar;

    [Space(20)]
    [Header("Choices Slider")]
    [SerializeField] private int _numberOfChoices;
    [SerializeField] private int _choiceOffset = 2;
    [SerializeField] private float _choiceHeight;
    [SerializeField] private Slider _slider;
    
    #endregion

    Coroutine textDrawing;

    void Start()
    {
        SetupInstance();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            DialogueManager.Instance.StopDialogue();
    }

    public void SetDialogueWindow(bool open)
    {
        _dialogueWindow.SetActive(open);

        if(open) 
            UIManager.Instance.AddWindow(this, false, true, true);
        else
            UIManager.Instance.RemoveWindow(this, false, true, true);
    }

    public void ChoicesSliderChange(float value)
    {
        float y = (value / 1f) * Mathf.Max(_numberOfChoices - _choiceOffset, 0) * _choiceHeight;
        _choicesParent.transform.position = new Vector2(_choicesParent.transform.position.x, y);
    }

    /// <summary>Draws text message and choices</summary>
    /// <param name="textMessage">The text message to display</param>
    /// <param name="choices">The choices to display</param>
    public void DrawNode(string textMessage, List<string> choices, string npcName = "")
    {
        if(_slider) _slider.value = 0f; // Reset slider value
        SetOutput(npcName, textMessage);
        SetInput(choices);
    }

    public void SetActions(bool active)
    {
        foreach(Button button in _interactionButtons)
            button.interactable = active;
    }

    private void SetOutput(string npcName, string text)
    {
        _outputBox.text = string.Empty;

        // Add Npc name if not empty
        if(npcName.Length > 0)
            _outputBox.text = npcName + ": ";

        // Add rest of message instantly or gradually
        if(text.Length > 0)
        {
            if(letterDrawTime == 0)
                _outputBox.text += text;
            else
            {
                if(textDrawing != null) StopCoroutine(textDrawing);
                textDrawing = StartCoroutine(DrawText(text));
            }
        }
    }

    private void SetInput(List<string> choices)
    {
        ClearInput();

        if(choices == null || choices.Count == 0) return;

        _numberOfChoices = choices.Count;
        int delta = _numberOfChoices - _choices.Count;
        
        //Creates necessary UI elements
        for (var i = 0; i < delta; i++)
            _choices.Add(CreateChoiceObject());

        //Sets up UI elements
        for (int j = 0; j < choices.Count; j++)
        {
            _choices[j].SetupChoice(choices[j], j);
            _choices[j].gameObject.SetActive(true);
        }
    }

    private void ClearInput()
    {
        for (var i = 0; i < _choices.Count; i++)
        {
            _choices[i].SetupChoice(string.Empty, i);
            _choices[i].gameObject.SetActive(false);
        }
    }

    public void ClearNode()
    {
        SetOutput(string.Empty, string.Empty);
        ClearInput();
    }

    private DialogueChoiceUI CreateChoiceObject()
    {
        GameObject go = Instantiate(_choicePrefab, _choicesParent);
        DialogueChoiceUI dc = go.GetComponent<DialogueChoiceUI>();

        return dc;
    }

    private IEnumerator DrawText(string text)
    {
        int i = 0;
        while(i < text.Length)
        {
            bool foundHTML = false;
            // Check for html codes for TMPro to handle
            if(text[i] == '<')
            {
                int j = i;
                string subString = "";
                // Search for closing tag
                while(j < text.Length)
                {
                    subString += text[j];
                    // If found closing tag, add substring and set new index
                    if(text[j] == '>')
                    {
                        foundHTML = true;
                        _outputBox.text += subString;
                        i = j + 1;
                        break;
                    }
                    j++;
                }
            }

            if(foundHTML == false)
            {  
                _outputBox.text += text[i];
                i++;
            }

            yield return new WaitForSeconds(letterDrawTime);
        }
    }

    public void UpdateBars(float like, float fear)
    {
        like *= likeBar.Length;
        fear *= fearBar.Length;
        
        for (var i = 0; i < likeBar.Length; i++)
            likeBar[i].SetActive(i < like);
        
        for (var i = 0; i < fearBar.Length; i++)
            fearBar[i].SetActive(i < fear);
    }
}