using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommand
{
    public string _id { get; private set;}
    public string _description { get; private set;}
    public string _format { get; private set;}

    public delegate void Command (string[] parameters);
    private Command _command;

    public DebugCommand(string id, string description, string format, Command command)
    {
        _id = id;
        _description = description;
        _format = format;

        _command = command;
    }

    public void Execute(string[] parameters)
    {
        _command.Invoke(parameters);
    }
}
