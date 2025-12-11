using System;
using UnityEngine;
using Zlipacket.CoreZlipacket.System.Command;

namespace TESTING
{
    public class TestCommand : MonoBehaviour
    {
        private void Start()
        {
            CommandManager.Instance.Excute("Print");
            CommandManager.Instance.Excute("Print_1p", "Hello World");
            CommandManager.Instance.Excute("Print_mp", "Line 1", "Line 2", "Line 3");
        }
    }
}