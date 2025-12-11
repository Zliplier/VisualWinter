using System.Collections.Generic;
using UnityEngine;
using Zlipacket.CoreZlipacket.Tools;
using Zlipacket.VNZlipacket.Dialogue;
using Zlipacket.VNZlipacket.ScriptableObjects;

namespace Zlipacket.VNZlipacket.Character
{
    public class CharacterVNManager : Singleton<CharacterVNManager>
    {
        private Dictionary<string, CharacterVN> characters = new Dictionary<string, CharacterVN>();
        
        private SO_CharacterVNConfig config => DialogueSystem.Instance.config.CharacterConfig;

        private const string CHARACTER_CASTING_ID = " as ";
        private const string CHARACTER_NAME_ID = "<charname>";
        private string characterRootPathFormat => $"Characters/{CHARACTER_NAME_ID}";
        public string characterPrefabNameFormat => $"Character - [{CHARACTER_NAME_ID}]";
        public string characterPrefabPathFormat => $"{characterRootPathFormat}/{characterPrefabNameFormat}";
        
        [SerializeField] private RectTransform _characterPanel = null;
        public RectTransform characterPanel => _characterPanel;
        
        public CharacterVNConfigData GetCharacterConfig(string name)
        {
            return config.GetConfig(name);
        }
        
        public CharacterVN GetCharacter(string name, bool createIfDoesntExisted = false)
        {
            if (characters.ContainsKey(name.ToLower()))
                return characters[name.ToLower()];
            else if (createIfDoesntExisted)
                return CreateCharacter(name);
            
            return null;
        }
        
        public CharacterVN CreateCharacter(string name)
        {
            if (characters.ContainsKey(name.ToLower()))
            {
                Debug.LogError("Character " + name + " is already registered");
                return null;
            }

            CharacterVNInfo info = GetCharacterInfo(name);
            CharacterVN character = CreateCharacterFromInfo(info);
            
            characters.Add(name.ToLower(), character);
            
            return character;
        }

        private CharacterVNInfo GetCharacterInfo(string name)
        {
            CharacterVNInfo result = new CharacterVNInfo();
            
            string[] nameData = name.Split(CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);
            
            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;
            result.config = config.GetConfig(result.castingName);
            result.prefab = GetPrefabForCharacter(result.castingName);
            result.rootCharacterFolder = FormatCharacterPath(characterRootPathFormat, result.castingName);
            
            return result;
        }

        private GameObject GetPrefabForCharacter(string name)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPathFormat, name);
            
            return Resources.Load<GameObject>(prefabPath);
        }

        public string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);

        private CharacterVN CreateCharacterFromInfo(CharacterVNInfo info)
        {
            CharacterVNConfigData config = info.config;

            switch (config.characterType)
            {
                case CharacterVN.CharacterType.Text:
                    return new CharacterText(name, config);
                
                case CharacterVN.CharacterType.Sprite:
                case CharacterVN.CharacterType.SpriteSheet:
                    return new CharacterSprite(name, config, info.prefab, info.rootCharacterFolder);
                
                case CharacterVN.CharacterType.Live2D:
                    return new CharacterLive2D(name, config, info.prefab, info.rootCharacterFolder);
                
                case CharacterVN.CharacterType.Model3D:
                    return new Character3DModel(name, config,  info.prefab, info.rootCharacterFolder);
                
                default:
                    return new CharacterText(name, config);
            }
        }
        
        private class CharacterVNInfo
        {
            public string name = "";
            public string castingName = "";
            
            public string rootCharacterFolder = "";
            
            public CharacterVNConfigData config = null;
            public GameObject prefab = null;
        }
    }
}