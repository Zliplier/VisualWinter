using System;
using TMPro;
using UnityEngine;

namespace Zlipacket.VNZlipacket.Dialogue.DialogueData
{
    [Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer NameContainer;
        public TextMeshProUGUI dialogueText;
        
        public void SetDialogueColor(Color color) => dialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;
    }
}