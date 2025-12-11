using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.System.Command.Database
{
    public class CommandDatabase
    {
        private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();

        public bool hasCommand(string commandName) => database.ContainsKey(commandName);

        public void AddCommand(string commandName, Delegate commandDelegate)
        {
            if (!database.TryAdd(commandName, commandDelegate))
                Debug.LogError($"Redundant database entries. Command already exists in database: {commandName}");
        }
        
        public Delegate GetCommand(string commandName)
        {
            if (!database.ContainsKey(commandName))
            {
                Debug.LogError($"Command: {commandName} not found in database.");
                return null;
            }
            
            return database[commandName];
        }
    }
}