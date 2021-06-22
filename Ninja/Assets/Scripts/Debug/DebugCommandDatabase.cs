using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugCommandDatabase
{
    public static Dictionary<string, DebugCommand> database = new Dictionary<string, DebugCommand>();

    public static void AddCommand(DebugCommand command)
    {
        if(HasCommand(command._id) == false)
            database[command._id] = command;
        else
            Debug.LogWarning("Command already exists: " + command._id);
    }

    public static DebugCommand GetCommand(string command)
    {
        if(HasCommand(command))
            return database[command];
        
        return null;
    }

    public static bool HasCommand(string command)
    {
        return database.ContainsKey(command);
    }

    public static bool ExecuteCommand(string command, string[] parameters)
    {
        DebugCommand dc = GetCommand(command);
        if(dc == null)
            return false;
        
        dc.Execute(parameters);
        return true;
    }
}
