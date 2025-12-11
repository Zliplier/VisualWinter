using System.Text.RegularExpressions;
using UnityEngine;
using Zlipacket.VNZlipacket.Dialogue.DialogueData;

namespace Zlipacket.VNZlipacket.Dialogue
{
    public class DialogueParser
    {
        private const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";
        
        public static DialogueLine Parse(string rawLine)
        {
            (string speakerName, string dialogue, string command) = RipContent(rawLine);
            
            return new DialogueLine(speakerName, dialogue, command);
        }

        private static (string, string, string) RipContent(string rawLine)
        {
            string speakerName = "", dialogue = "", command = "";
            
            //Find Dialogue
            int dialogueStartIndex = -1;
            int dialogueEndIndex = -1;
            bool isEscaped = false;

            for (int i = 0; i < rawLine.Length; i++)
            {
                char currentChar = rawLine[i];
                
                if (currentChar == '\\')
                {
                    isEscaped = !isEscaped;
                }
                else if (currentChar == '"' && !isEscaped)
                {
                    if (dialogueStartIndex == -1)
                        dialogueStartIndex = i;
                    else if (dialogueEndIndex == -1)
                    {
                        dialogueEndIndex = i;
                        break;
                    }
                }
                else
                {
                    isEscaped = false;
                }
            }
            
            //Find Command Pattern
            Regex commandRegex = new Regex(commandRegexPattern);
            MatchCollection matches = commandRegex.Matches(rawLine);
            int commandStartIndex = -1;

            foreach (Match match in matches)
            {
                if (match.Index < dialogueStartIndex || match.Index > dialogueEndIndex)
                {
                    commandStartIndex = match.Index;
                    break;
                }
            }

            if (commandStartIndex != -1 && (dialogueStartIndex == -1 && dialogueEndIndex == -1))
                return ("", "", rawLine.Trim());
            
            //If we are here, we either have dialogue or multi argument command. Figure out if this is dialogue.
            if (dialogueStartIndex != -1 && dialogueEndIndex != -1 &&
                (commandStartIndex == -1 || commandStartIndex > dialogueEndIndex))
            {
                //Valid Dialogue
                speakerName = rawLine.Substring(0, dialogueStartIndex).Trim();
                dialogue = rawLine.Substring(dialogueStartIndex + 1, dialogueEndIndex - dialogueStartIndex - 1).Replace("\\\"", "\"");
                if (commandStartIndex != -1)
                    command = rawLine.Substring(commandStartIndex).Trim();
            }
            else if (commandStartIndex != -1 && commandStartIndex > dialogueStartIndex)
            {
                command = rawLine;
            }
            else
            {
                dialogue = rawLine;
            }
            
            return (speakerName, dialogue, command);
        }
    }
}