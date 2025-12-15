using UnityEngine;
using Zlipacket.VNZlipacket.Character;

namespace Zlipacket.VNZlipacket.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CharacterVNConfig", menuName = "VNZlipacket/CharacterVN Config")]
    public class SO_CharacterVNConfig : ScriptableObject
    {
        public CharacterConfigData[] characters;

        public CharacterConfigData GetConfig(string name)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                CharacterConfigData data = characters[i];

                if (string.Equals(data.name.ToLower(), name.ToLower()) || string.Equals(data.alias.ToLower(), name.ToLower()))
                {
                    return data.Copy();
                }
            }
            
            return CharacterConfigData.DefaultData;
        }
    }
}