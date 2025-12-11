using TMPro;
using UnityEngine;

namespace Zlipacket.VNZlipacket.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DialogueSystemConfig", menuName = "VNZlipacket/Dialogue System Config")]
    public class SO_DialogueSystemConfig : ScriptableObject
    {
        public SO_CharacterVNConfig CharacterConfig;
        
        public Color defaultTextColor = Color.black;
        public TMP_FontAsset defaultFont;
    }
}