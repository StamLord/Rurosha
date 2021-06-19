using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private bool showConsole;
    [SerializeField] private GameObject consoleObject;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private CharacterStats playerStats;

    List<DebugCommand> commandList = new List<DebugCommand>();

    [SerializeField] private List<string> commandHistory = new List<string>();
    [SerializeField] private int historyIndex;

    private void Awake() 
    {
        commandList.Add(new DebugCommand(
            "rosebud", 
            "Gives the player 1000 gold.", 
            "rosebud", 
            (string[] paramaters) => {
                Debug.Log("Invoking rosebud! You rascal!");
            }));

        commandList.Add(new DebugCommand(
            "increase_attribute", 
            "Increases attribue", 
            "increase_attribute <attribute name>", 
            (string[] paramaters) => {
                playerStats.IncreaseAttribute(paramaters[0]);
            }));

        commandList.Add(new DebugCommand(
            "set_attribute", 
            "Sets attribue", 
            "set_attribute <attribute name> <level>", 
            (string[] paramaters) => {
                playerStats.SetAttributeLevel(paramaters[0], Int32.Parse(paramaters[1]));
            }));
    }
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))    
            ShowConsole(!showConsole);

        if(showConsole)
        {
            if(Input.GetKeyDown(KeyCode.Return))    
                SubmitInput();
            
            if(Input.GetKeyDown(KeyCode.UpArrow))    
            {
                historyIndex = Mathf.Max(0, historyIndex - 1);
                inputField.text = commandHistory[historyIndex];
            }

            if(Input.GetKeyDown(KeyCode.DownArrow))    
            {
                historyIndex = Mathf.Min(historyIndex + 1, commandHistory.Count);
                inputField.text = (historyIndex < commandHistory.Count)? commandHistory[historyIndex] : "";
            }
        }
    }

    void ShowConsole(bool on)
    {
        showConsole = on;
        consoleObject.SetActive(on);
        if(inputField.isFocused == false) inputField.Select();
    }

    public void SubmitInput()
    {
        string input = inputField.text;

        if(input.Length < 1)
            return;

        string[] split = input.Split(' ');
        string[] parameters = new string[0];

        if(split.Length > 1)
        {
            parameters = new string[split.Length - 1];
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = split[i + 1];
        }

        foreach(DebugCommand dc in commandList)
        {
            if(dc._id == split[0])
                dc.Invoke(parameters);
        }

        commandHistory.Add(inputField.text);
        historyIndex = commandHistory.Count;

        inputField.text = "";
    }

}

public class DebugCommand
{
    public string _id { get; private set;}
    public string _description { get; private set;}
    public string _format { get; private set;}

    private UnityAction<string[]> _command;

    public DebugCommand(string id, string description, string format, UnityAction<string[]> command)
    {
        _id = id;
        _description = description;
        _format = format;

        _command = command;
    }

    public void Invoke(string[] parameters)
    {
        _command.Invoke(parameters);
    }
}