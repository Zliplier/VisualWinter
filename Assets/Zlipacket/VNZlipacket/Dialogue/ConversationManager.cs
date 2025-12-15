using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zlipacket.CoreZlipacket.System.Command;
using Zlipacket.CoreZlipacket.Tools;
using Zlipacket.VNZlipacket.Character;
using Zlipacket.VNZlipacket.Dialogue.DialogueData;

namespace Zlipacket.VNZlipacket.Dialogue
{
    public class ConversationManager
    {
        public const string COMMENT_ID = "//";
        
        private DialogueSystem dialogueSystem = DialogueSystem.Instance;
        private TextArchitect architect = null;
        private Coroutine process = null;
        public bool isRunning => process != null;
        
        bool userPrompt = false;
        
        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            DialogueSystem.Instance.onUserPromptNext.AddListener(OnUserPromptNext);
        }

        private void OnUserPromptNext()
        {
            userPrompt = true;
        }
        
        public Coroutine StartConversation(List<string> conversation)
        {
            StopConversation();

            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
            
            return process;
        }

        public void StopConversation()
        {
            if (!isRunning)
                return;
            
            dialogueSystem.StopCoroutine(process);
            process = null;
        }
        
        IEnumerator RunningConversation(List<string> conversation)
        {
            for (int i = 0; i < conversation.Count; i++)
            {
                //Skip Blank line or Comment
                if (string.IsNullOrWhiteSpace(conversation[i]) || conversation[i].StartsWith(COMMENT_ID))
                    continue;
                
                DialogueLine line = DialogueParser.Parse(conversation[i]);
                
                //Show Dialogue
                if (line.hasDialogue)
                    yield return Line_RunDialogue(line);
                
                //Run any Command
                if (line.hasCommand)
                    yield return Line_RunCommands(line);

                if (line.hasDialogue)
                {
                    yield return WaitForUserInput();

                    CommandManager.Instance.StopAllProcess();
                }
            }
        }
        
        IEnumerator Line_RunDialogue(DialogueLine line)
        {
            if (line.hasSpeaker)
                HandleSpeakerLogic(line.speakerData);
            
            //Build Dialogue
            yield return BuildLineSegment(line.dialogueInfo);
        }
        
        private void HandleSpeakerLogic(SpeakerData speakerData)
        {
            bool characterMustBeCreated = speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpression;
            
            VN_Character vnCharacter = VN_CharacterManager.Instance.GetCharacter(speakerData.name, createIfDoesntExisted: characterMustBeCreated);

            if (speakerData.makeCharacterEnter && (!vnCharacter.isVisible && !vnCharacter.isRevealing))
                vnCharacter.Show();
            
            dialogueSystem.ShowSpeakerName(speakerData.displayName);
            dialogueSystem.ApplySpeakerDataToDialogueContainer(speakerData.name);
            
            if (speakerData.isCastingPosition)
                vnCharacter.MoveToPosition(speakerData.castPosition);

            if (speakerData.isCastingExpression)
            {
                foreach (var ce in speakerData.CastExpressions)
                    vnCharacter.OnRecieveCastingExpression(ce.layer, ce.expression);
            }
        }
        
        IEnumerator Line_RunCommands(DialogueLine line)
        {
            List<CommandData.Command> commands = line.commandData.commands;

            foreach (CommandData.Command command in commands)
            {
                //Debug.Log($"Run Command: {command.name}, Arguments: {command.arguments}");
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.Instance.Excute(command.name, command.arguments);
                    while (!cw.isDone)
                    {
                        if (userPrompt)
                        {
                            CommandManager.Instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                    CommandManager.Instance.Excute(command.name, command.arguments);
            }
        }
        
        IEnumerator BuildLineSegment(DialogueInfo line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DialogueInfo.DialogueSegment segment = line.segments[i];
                
                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);
                
                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DialogueInfo.DialogueSegment segment)
        {
            switch (segment.startSignal)
            {
                case DialogueInfo.DialogueSegment.StartSignal.Clear:
                    break;
                case DialogueInfo.DialogueSegment.StartSignal.Append:
                    yield return WaitForUserInput();
                    break;
                case DialogueInfo.DialogueSegment.StartSignal.WaitClear:
                    break;
                case DialogueInfo.DialogueSegment.StartSignal.WaitAppend:
                    yield return new WaitForSeconds(segment.signalDelay);
                    break;
                default:
                    break;
            }
        }
        
        IEnumerator BuildDialogue(string line, bool append = false)
        {
            if (!append)
                architect.Build(line);
            else
                architect.Append(line);
            
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();
                    
                    userPrompt = false;
                }
                
                yield return null;
            }
        }

        IEnumerator WaitForUserInput()
        {
            while (!userPrompt)
                yield return null;
            
            userPrompt = false;
        }
    }
}