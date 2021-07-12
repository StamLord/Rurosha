using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DebugConsole : UIWindow
{
    [Header("Self References")]
    [SerializeField] private bool showConsole;
    [SerializeField] private GameObject consoleObject;
    [SerializeField] private TMP_InputField inputField;

    [Header("Other References")]
    [SerializeField] private InputDebugWindow inputDebugWindow;

    //List<DebugCommand> commandList = new List<DebugCommand>();

    [Header("Command History")]
    [SerializeField] private List<string> commandHistory = new List<string>();
    [SerializeField] private int historyIndex;

    private void Awake() 
    {
        DebugCommandDatabase.AddCommand(new DebugCommand(
            "rosebud", 
            "Gives the player 1000 gold.", 
            "rosebud", 
            (string[] paramaters) => {
                Debug.Log("Invoking rosebud! You rascal!");
            }));

        DebugCommandDatabase.AddCommand(new DebugCommand(
            "inputdebug", 
            "Turns Input Debug window On / Off", 
            "inputdebug <on/off>", 
            (string[] paramaters) => {
                string p = paramaters[0].ToLower();
                if(p == "on")
                    inputDebugWindow.Show(true);
                else if(p == "off")
                    inputDebugWindow.Show(false);
            }));

        DebugCommandDatabase.AddCommand(new DebugCommand(
            "spawn", 
            "Spawns an object", 
            "spawn <objectname> [optional]<amount>", 
            (string[] paramaters) => {
                // To Add
            }));
    }
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))    
            ShowConsole(!showConsole);

        if(showConsole && inputField.isFocused == false)
            SelectInputField();

        if(showConsole)
        {
            if(Input.GetKeyDown(KeyCode.Return))    
                SubmitInput();
            
            if(Input.GetKeyDown(KeyCode.UpArrow))    
            {
                historyIndex = Mathf.Max(0, historyIndex - 1);
                inputField.text = commandHistory[historyIndex];
                inputField.caretPosition = inputField.text.Length;
            }

            if(Input.GetKeyDown(KeyCode.DownArrow))    
            {
                historyIndex = Mathf.Min(historyIndex + 1, commandHistory.Count);
                inputField.text = (historyIndex < commandHistory.Count)? commandHistory[historyIndex] : "";
                inputField.caretPosition = inputField.text.Length;
            }
        }
    }

    void ShowConsole(bool on)
    {
        showConsole = on;
        consoleObject.SetActive(on);

        if(on)
            UIManager.Instance.AddWindow(this, true, false);
        else
            UIManager.Instance.RemoveWindow(this, true, false);
        
        // Make sure we can type in field
        if(inputField.isFocused == false)
            SelectInputField();
    }

    void SelectInputField()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void SubmitInput()
    {
        string input = inputField.text;

        if(input.Length < 1)
            return;

        string[] command = input.Split(' ');
        string[] parameters = new string[0];

        if(command.Length > 1)
        {
            parameters = new string[command.Length - 1];
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = command[i + 1];
        }

        DebugCommandDatabase.ExecuteCommand(command[0], parameters);

        commandHistory.Add(inputField.text);
        historyIndex = commandHistory.Count;

        inputField.text = "";
    }
}