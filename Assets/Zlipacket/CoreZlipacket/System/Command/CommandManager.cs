using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zlipacket.CoreZlipacket.System.Command.Database;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.System.Command
{
    public class CommandManager : Singleton<CommandManager>
    {
        private CommandDatabase database;
        private static Coroutine process = null;
        public static bool isRunningProcess => process != null;
        
        public override void Awake()
        {
            base.Awake();
            database = new CommandDatabase();
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();

            foreach (Type type in extensionTypes)
            {
                MethodInfo extendMethod = type.GetMethod("Extend");
                extendMethod.Invoke(null, new object[] { database });
            }
        }

        public Coroutine Excute(string commandName, params string[] args)
        {
            Delegate command = database.GetCommand(commandName);
            
            if (command == null)
                return null;

            return StartProcess(commandName, command, args);
        }

        private Coroutine StartProcess(string commandName, Delegate command, params string[] args)
        {
            StopCurrentProcess();

            process = StartCoroutine(RunningProcess(command, args));

            return process;
        }

        private void StopCurrentProcess()
        {
            if (process != null)
                StopCoroutine(process);
            
            process = null;
        }

        private IEnumerator RunningProcess(Delegate command, string[] args)
        {
            yield return WaitForProcessToComplete(command, args);
            
            process = null;
        }

        private IEnumerator WaitForProcessToComplete(Delegate command, string[] args)
        {
            if (command is Action)
                command.DynamicInvoke();
            
            else if(command is Action<string>)
                command.DynamicInvoke(args[0]);
            
            else if(command is Action<string[]>)
                command.DynamicInvoke((object)args);
            
            else if (command is Func<IEnumerator>)
                yield return ((Func<IEnumerator>)command)();
            
            else if (command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args[0]);
            
            else if (command is Func<string[], IEnumerator>)
                yield return ((Func<string[], IEnumerator>)command)(args);
        }
    }
}