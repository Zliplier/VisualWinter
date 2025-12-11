using System;
using System.Collections.Generic;
using UnityEngine;
using Zlipacket.CoreZlipacket.System.IO;
using Zlipacket.VNZlipacket.Dialogue;

namespace TESTING
{
    public class TestConversationFiles : MonoBehaviour
    {
        [SerializeField] private TextAsset textFile;
        
        private void Start()
        {
            StartConversation();
        }

        public void StartConversation()
        {
            List<string> lines = FileManager.ReadTextAsset(textFile, false);
            
            DialogueSystem.Instance.Say(lines);
        }
    }
}