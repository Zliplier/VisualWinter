using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Zlipacket.VNZlipacket.Dialogue.DialogueData
{
    public class DialogueInfo
    {
        public List<DialogueSegment> segments;
        private const string segmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";
        
        public DialogueInfo(string rawDialogue)
        {
            segments = RipSegments(rawDialogue);
        }

        public List<DialogueSegment> RipSegments(string rawDialogue)
        {
            List<DialogueSegment> segments = new List<DialogueSegment>();
            MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPattern);
            
            int lastIndex = 0;
            //Find the first or only signal on the file
            DialogueSegment segment = new DialogueSegment();
            segment.dialogue = (matches.Count == 0) ? rawDialogue : rawDialogue.Substring(0, matches[0].Index);
            segment.startSignal = DialogueSegment.StartSignal.None;
            segment.signalDelay = 0f;
            segments.Add(segment);
            
            if (matches.Count == 0)
                return segments;
            else
                lastIndex = matches[0].Index;

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                segment = new DialogueSegment();
                
                string signalMatch = match.Value;//{A}
                signalMatch = signalMatch.Substring(1, signalMatch.Length - 2);
                string[] signalSplit = signalMatch.Split(' ');
                
                segment.startSignal = DialogueSegment.ParseShortHandStartSignal(signalSplit[0]);
                
                //Get Signal Delay
                if (signalSplit.Length > 1)
                    float.TryParse(signalSplit[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out segment.signalDelay);
                    //float.TryParse(signalSplit[1], out segment.signalDelay);
                
                //Get dialogue from the segment
                int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
                segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
                lastIndex = nextIndex;
                
                segments.Add(segment);
            }
            
            return segments;
        }
        
        public struct DialogueSegment
        {
            public string dialogue;
            public StartSignal startSignal;
            public float signalDelay;
            public bool appendText => startSignal == StartSignal.Append || startSignal == StartSignal.WaitAppend;
            public enum StartSignal { None, Clear, Append, WaitClear, WaitAppend }

            public static StartSignal ParseShortHandStartSignal(string signalShortHand)
            {
                switch (signalShortHand.ToUpper())
                {
                    case "C":
                        return StartSignal.Clear;
                    case "A":
                        return StartSignal.Append;
                    case "WC":
                        return StartSignal.WaitClear;
                    case "WA":
                        return StartSignal.WaitAppend;
                    default:
                        return StartSignal.None;
                }
            }
        }
    }
}