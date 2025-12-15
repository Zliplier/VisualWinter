using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Internal;
using Zlipacket.CoreZlipacket.System.Command;
using Zlipacket.CoreZlipacket.System.Command.Database;
using Zlipacket.CoreZlipacket.Tools;
using Zlipacket.VNZlipacket.Character;

namespace Zlipacket.VNZlipacket.Command
{
    public class CMD_VN_Character : CMD_DatabaseExtension
    {
        private static string[] PARAM_ENABLE = { "-e", "-enable" };
        private static string[] PARAM_IMMEDIATE = { "-i", "-immediate" };
        private static string[] PARAM_SPEED = { "-s", "-spd", "-speed" };
        private static string[] PARAM_SMOOTH = { "-sm", "-smooth" };
        private static string[] PARAM_CHARACTER = { "-ch", "-character" };
        private static string[] PARAM_COLOR = { "-c", "-color" };
        private static string[] PARAM_LAYER = { "-l", "-layer" };
        private static string PARAM_XPOS => "-x";
        private static string PARAM_YPOS => "-y";
        
        public new static void Extend(CommandDatabase database)
        {
            database.AddCommand("CreateCharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("MoveCharacter", new Func<string[], IEnumerator>(MoveCharacter));
            database.AddCommand("Show", new Func<string[], IEnumerator>(ShowAll));
            database.AddCommand("Hide", new Func<string[], IEnumerator>(HideAll));
            database.AddCommand("SetColor", new Func<string[], IEnumerator>(SetColor));
            database.AddCommand("SetPriority", new Action<string[]>(SetPriority));
            database.AddCommand("Highlight", new Func<string[], IEnumerator>(Highlight));
            database.AddCommand("UnHighlight", new Func<string[], IEnumerator>(UnHighlight));
            database.AddCommand("ChangeExpression", new Func<string[], IEnumerator>(ChangeExpression));
        }

        public static void CreateCharacter(string[] data)
        {
            string characterName;
            bool enable = false;
            bool immediate = false;
            
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_ENABLE, out enable, defaultValue: false);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_CHARACTER, out characterName, defaultValue: data[0]);
            
            VN_Character character = VN_CharacterManager.Instance.CreateCharacter(characterName);
            
            //Choose if we want to show the character on creation.
            if (!enable)
                return;
            
            //Show Character with transition or not.
            if (!immediate)
                character.isVisible = true;
            else
                character.Show();
        }

        public static IEnumerator MoveCharacter(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            
            parameters.TryGetValue(PARAM_CHARACTER, out string characterName, defaultValue: data[0]);
            VN_Character character = VN_CharacterManager.Instance.GetCharacter(characterName);
            
            if (character == null)
                yield break;

            float x = 0, y = 0;
            float speed = 1f;
            bool smooth = false;
            bool immediate = false;

            parameters.TryGetValue(PARAM_XPOS, out x);
            parameters.TryGetValue(PARAM_YPOS, out y);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_SMOOTH, out smooth, defaultValue: false);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            
            Vector2 position = new Vector2(x, y);
            if (immediate)
                character.SetPosition(position);
            else
            {
                commandManager.AddTerminationActionToCurrentProcess(() =>
                {
                    character?.SetPosition(position);
                });
                yield return character.MoveToPosition(position, speed, smooth);
            }
        }
        
        public static IEnumerator ShowAll(string[] data)
        {
            List<VN_Character> characters = new();
            float speed = 1f;
            bool immediate = false;
            
            foreach (string s in data)
            {
                VN_Character character = VN_CharacterManager.Instance.GetCharacter(s, createIfDoesntExisted: false);
                if (character != null)
                    characters.Add(character);
            }
            
            if (characters.Count == 0)
                yield break;

            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            foreach (VN_Character character in characters)
            {
                if (!immediate)
                    character.isVisible = true;
                else
                    character.Show(speed);
            }

            if (!immediate)
            {
                commandManager.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.isVisible = true;
                });
                
                while (characters.Any(c => c.isRevealing))
                    yield return null;
            }
        }
        
        public static IEnumerator HideAll(string[] data)
        {
            List<VN_Character> characters = new();
            float speed = 1f;
            bool immediate = false;
            
            foreach (string s in data)
            {
                VN_Character character = VN_CharacterManager.Instance.GetCharacter(s, createIfDoesntExisted: false);
                if (character != null)
                    characters.Add(character);
            }
            
            if (characters.Count == 0)
                yield break;

            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            foreach (VN_Character character in characters)
            {
                if (!immediate)
                    character.isVisible = false;
                else
                    character.Hide(speed);
            }

            if (!immediate)
            {
                commandManager.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.isVisible = false;
                });
                
                while (characters.Any(c => c.isHiding))
                    yield return null;
            }
        }
        
        public static IEnumerator SetColor(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            
            parameters.TryGetValue(PARAM_CHARACTER, out string characterName, defaultValue: data[0]);
            VN_Character character = VN_CharacterManager.Instance.GetCharacter(characterName);
            
            if (character == null)
            {
                Debug.LogError($"Character {characterName} does not exist to set Color.");
                yield break;
            }
            
            float speed = 1f;
            bool immediate = false;
            string colorName = "";
            
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_COLOR, out colorName, defaultValue: "Black");

            Color color = ColorExtension.GetColorFromName(colorName);
            
            if (immediate)
                character.SetColor(color);
            else
            {
                commandManager.AddTerminationActionToCurrentProcess(() =>
                {
                    character?.SetColor(color);
                });
                yield return character.TransitionToColor(color, speed);
            }
        }
        
        public static void SetPriority(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            
            parameters.TryGetValue(PARAM_CHARACTER, out string characterName, defaultValue: data[0]);
            parameters.TryGetValue(PARAM_LAYER, out int layer, defaultValue: 0);
            
            VN_Character character = VN_CharacterManager.Instance.GetCharacter(characterName);

            if (character == null)
            {
                Debug.LogError($"Character {characterName} does not exist to set Priority.");
                return;
            }
            
            character.SetPriority(layer);
        }
        
        public static IEnumerator Highlight(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_CHARACTER, out string characterName, defaultValue: data[0]);
            parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);
            
            VN_Character character = VN_CharacterManager.Instance.GetCharacter(characterName);

            if (character == null)
            {
                Debug.LogError($"Character {characterName} does not exist to Highlight.");
                yield break;
            }
            
            if (immediate)
                character.Highlight(speed, true);
            else
            {
                commandManager.AddTerminationActionToCurrentProcess(() =>
                {
                    character.Highlight(speed, true);
                });
                yield return character.Highlight(speed, false);;
            }
        }
        
        public static IEnumerator UnHighlight(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_CHARACTER, out string characterName, defaultValue: data[0]);
            parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);
            
            VN_Character character = VN_CharacterManager.Instance.GetCharacter(characterName);

            if (character == null)
            {
                Debug.LogError($"Character {characterName} does not exist to Highlight.");
                yield break;
            }
            
            if (immediate)
                character.UnHightlight(speed, true);
            else
            {
                commandManager.AddTerminationActionToCurrentProcess(() =>
                {
                    character.UnHightlight(speed, true);
                });
                yield return character.UnHightlight(speed, false);;
            }
        }
        
        public static IEnumerator ChangeExpression(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_CHARACTER, out string characterName, defaultValue: data[0]);
            parameters.TryGetValue(new string[] { "-ex", "-expression" }, out string expression, defaultValue: "");
            parameters.TryGetValue(PARAM_LAYER, out int layer, defaultValue: 0);
            parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);
            
            VN_Character character = VN_CharacterManager.Instance.GetCharacter(characterName);

            if (character == null)
            {
                Debug.LogError($"Character {characterName} does not exist to change Expression.");
                yield break;
            }
            
            if (immediate)
                character.OnRecieveCastingExpression(layer, expression, true);
            else
            {
                commandManager.AddTerminationActionToCurrentProcess(() =>
                {
                    character.OnRecieveCastingExpression(layer, expression, true);
                });
                yield return character.OnRecieveCastingExpression(layer, expression, false);
            }
        }
    }
}