using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zlipacket.CoreZlipacket.Tools;
using Zlipacket.VNZlipacket.Dialogue;
using Zlipacket.VNZlipacket.ScriptableObjects;

namespace Zlipacket.VNZlipacket.Character
{
    public class VN_CharacterManager : Singleton<VN_CharacterManager>
    {
        private Dictionary<string, VN_Character> characters = new Dictionary<string, VN_Character>();
        
        private SO_CharacterVNConfig config => DialogueSystem.Instance.config.CharacterConfig;

        private const string CHARACTER_CASTING_ID = " as ";
        private const string CHARACTER_NAME_ID = "<charname>";
        private string characterRootPathFormat => $"Characters/{CHARACTER_NAME_ID}";
        public string characterPrefabNameFormat => $"Character - [{CHARACTER_NAME_ID}]";
        public string characterPrefabPathFormat => $"{characterRootPathFormat}/{characterPrefabNameFormat}";
        
        [SerializeField] private RectTransform _characterPanel = null;
        public RectTransform characterPanel => _characterPanel;
        
        public CharacterConfigData GetCharacterConfig(string name)
        {
            return config.GetConfig(name);
        }
        
        public VN_Character GetCharacter(string name, bool createIfDoesntExisted = false)
        {
            if (characters.ContainsKey(name.ToLower()))
                return characters[name.ToLower()];
            else if (createIfDoesntExisted)
                return CreateCharacter(name);
            
            return null;
        }
        
        public bool HasCharacter(string name) => characters.ContainsKey(name.ToLower());
        
        public VN_Character CreateCharacter(string name, bool revealAfterCreation = false)
        {
            if (characters.ContainsKey(name.ToLower()))
            {
                Debug.LogError("Character " + name + " is already registered");
                return null;
            }

            CharacterVNInfo info = GetCharacterInfo(name);
            VN_Character character = CreateCharacterFromInfo(info);
            
            characters.Add(name.ToLower(), character);

            if (revealAfterCreation)
                character.Show();
            
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

        private VN_Character CreateCharacterFromInfo(CharacterVNInfo info)
        {
            CharacterConfigData config = info.config;

            switch (config.characterType)
            {
                case VN_Character.CharacterType.Text:
                    return new CharacterText(name, config);
                
                case VN_Character.CharacterType.Sprite:
                case VN_Character.CharacterType.SpriteSheet:
                    return new CharacterSprite(name, config, info.prefab, info.rootCharacterFolder);
                
                case VN_Character.CharacterType.Live2D:
                    return new CharacterLive2D(name, config, info.prefab, info.rootCharacterFolder);
                
                case VN_Character.CharacterType.Model3D:
                    return new Character3DModel(name, config,  info.prefab, info.rootCharacterFolder);
                
                default:
                    return new CharacterText(name, config);
            }
        }

        public void SortCharacters()
        {
            List<VN_Character> activeCharacters = characters.Values.
                Where(c => c.root.gameObject.activeInHierarchy && c.isVisible).ToList();
            List<VN_Character> inActiveCharacters = characters.Values.
                Except(activeCharacters).ToList();
            
            activeCharacters.Sort((c1, c2) => c1.priority.CompareTo(c2.priority));
            activeCharacters.Concat(inActiveCharacters);
            
            SortCharacters(activeCharacters);
        }

        public void SortCharacters(string[] names)
        {
            List<VN_Character> sortedCharacters = new List<VN_Character>();
            sortedCharacters = names
                .Select(name => GetCharacter(name))
                .Where(character => character != null)
                .ToList();

            List<VN_Character> remainingCharacters = characters.Values
                .Except(sortedCharacters)
                .OrderBy(character => character.priority)
                .ToList();
            
            sortedCharacters.Reverse();
            
            int startingPriority = remainingCharacters.Count > 0 ? remainingCharacters.Max(character => character.priority) : 0;
            for (int i = 0; i < remainingCharacters.Count; i++)
            {
                VN_Character vnCharacter = remainingCharacters[i];
                vnCharacter.SetPriority(startingPriority + i + 1);
            }
            
            remainingCharacters.Concat(sortedCharacters); //Convert to all Characters.
            SortCharacters(sortedCharacters);
        }
        
        private void SortCharacters(List<VN_Character> charactersSortingOrder)
        {
            int i = 0;
            foreach (VN_Character character in charactersSortingOrder)
            {
                character.root.SetSiblingIndex(i++);    
            }
        }
        
        private class CharacterVNInfo
        {
            public string name = "";
            public string castingName = "";
            
            public string rootCharacterFolder = "";
            
            public CharacterConfigData config = null;
            public GameObject prefab = null;
        }
    }
}