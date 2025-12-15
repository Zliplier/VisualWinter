using UnityEngine;

namespace Zlipacket.VNZlipacket.Character
{
    public class Character3DModel : VN_Character
    {
        public Character3DModel(string name, CharacterConfigData config, GameObject prefab, string rootCharacterFolder) : 
            base(name, config, prefab)
        {
        }
    }
}