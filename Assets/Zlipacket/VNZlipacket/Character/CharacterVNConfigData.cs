using System;
using TMPro;
using UnityEngine;
using Zlipacket.VNZlipacket.Dialogue;

namespace Zlipacket.VNZlipacket.Character
{
    [Serializable]
    public class CharacterVNConfigData
    {
        public string name;
        public string alias;
        public CharacterVN.CharacterType characterType;

        public Color nameColor;
        public Color dialogueColor;
        
        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public CharacterVNConfigData Copy()
        {
            CharacterVNConfigData result = new CharacterVNConfigData();
            
            result.name = name;
            result.alias = alias;
            result.characterType = characterType;
            
            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;
            
            result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
            result.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);
            
            return result;
        }
        
        private static Color defaultTextColor => DialogueSystem.Instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont => DialogueSystem.Instance.config.defaultFont;
        public static CharacterVNConfigData DefaultData
        {
            get
            {
                CharacterVNConfigData result = new CharacterVNConfigData();
            
                result.name = "";
                result.alias = "";
                result.characterType = CharacterVN.CharacterType.Text;
                
                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;
            
                result.nameColor = new Color(defaultTextColor.r, defaultTextColor.g, defaultTextColor.b, defaultTextColor.a);
                result.dialogueColor = new Color(defaultTextColor.r, defaultTextColor.g, defaultTextColor.b, defaultTextColor.a);
                
                return result;
            }
        }
    }
}