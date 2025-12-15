using Zlipacket.CoreZlipacket.System.Command;

namespace Zlipacket.VNZlipacket.Dialogue.DialogueData
{
    public class DialogueLine
    {
        public SpeakerData speakerData;
        public DialogueInfo dialogueInfo;
        public CommandData commandData;
        
        public bool hasSpeaker => speakerData != null;
        public bool hasDialogue => dialogueInfo != null;
        public bool hasCommand => commandData != null;
        
        public DialogueLine(string speaker, string dialogue, string command)
        {
            this.speakerData = string.IsNullOrWhiteSpace(speaker) ? null : new SpeakerData(speaker);
            this.dialogueInfo = string.IsNullOrWhiteSpace(dialogue) ? null : new DialogueInfo(dialogue);
            this.commandData = string.IsNullOrWhiteSpace(command) ? null : new CommandData(command);
        }
    }
}