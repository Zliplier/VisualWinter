using System;
using UnityEngine;
using Zlipacket.VNZlipacket.Dialogue;
using Zlipacket.VNZlipacket.Dialogue.DialogueData;

namespace TESTING
{
    public class TestParsing : MonoBehaviour
    {
        private string rawLine = "Haru \"Dialogue goes here\" TestCommand(\"argument\")";
        
        private void Start()
        {
            DialogueLine line = DialogueParser.Parse(rawLine);
            
            Debug.Log($"Speaker: {line.speakerData}, Dialogue: {line.dialogueInfo}, Command: {line.commandData}");
        }
    }
}