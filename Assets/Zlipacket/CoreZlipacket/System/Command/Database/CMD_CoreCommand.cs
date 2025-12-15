using System;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.System.Command.Database
{
    public class CMD_CoreCommand : CMD_DatabaseExtension
    {
        public new static void Extend(CommandDatabase database)
        {
            database.AddCommand("Print", new Action<string>(Print));
        }

        public static void Print(string data)
        {
            Debug.Log(data);
        }
        
    }
}