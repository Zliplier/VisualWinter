using System;
using TMPro;
using UnityEngine;
using Zlipacket.VNZlipacket.Dialogue;

namespace Zlipacket.VNZlipacket.Character
{
    [Serializable]
    public class CharacterConfigData
    {
        public string name;
        public string alias;
        public VN_Character.CharacterType characterType;

        public Color nameColor;
        public Color dialogueColor;
        
        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();
            
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
        public static CharacterConfigData DefaultData
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();
            
                result.name = "";
                result.alias = "";
                result.characterType = VN_Character.CharacterType.Text;
                
                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;
            
                result.nameColor = new Color(defaultTextColor.r, defaultTextColor.g, defaultTextColor.b, defaultTextColor.a);
                result.dialogueColor = new Color(defaultTextColor.r, defaultTextColor.g, defaultTextColor.b, defaultTextColor.a);
                
                return result;
            }
        }
    }
}