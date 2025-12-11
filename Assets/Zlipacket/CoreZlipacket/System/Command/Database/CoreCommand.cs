using System;
using System.Collections;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.System.Command.Database
{
    public class CoreCommand : CMD_DatabaseExtension
    {
        public new static void Extend(CommandDatabase database)
        {
            database.AddCommand("Print", new Action(PrintDefaultMessage));
            database.AddCommand("Print_1p", new Action<string>(PrintUserMessage));
            database.AddCommand("Print_mp", new Action<string[]>(PrintMultipleLines));
            
            //Add Lambda with no Parameters.
            
            
            //Add Coroutine with no Parameters.
            database.AddCommand("SimpleProcess", new Func<IEnumerator>(SimpleProcess));
            
        }

        private static void PrintDefaultMessage()
        {
            Debug.Log("Print Message to Console.");
        }

        private static void PrintUserMessage(string message)
        {
            Debug.Log($"User Message: {message}");
        }
        
        private static void PrintMultipleLines(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                Debug.Log($"{i} : {lines[i]}");
            }
        }

        private static IEnumerator SimpleProcess()
        {
            for (int i = 0; i < 3; i++)
            {
                Debug.Log(i);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}