using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.VNZlipacket.Character
{
    public class CharacterSprite : CharacterVN
    {
        private const string SPRITE_RENDERER_PARENT_NAME = "Renderers";
        private const string SPRITE_DEFAULT_TEXTURE_NAME = "Default";
        
        public float showHideSpeed = 3f;
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();
        
        public List<CharacterSpriteLayer> spriteLayers = new List<CharacterSpriteLayer>();

        private string artAssetsDirectory = "";
        
        public CharacterSprite(string name, CharacterVNConfigData config, GameObject prefab, string rootCharacterFolder) : base(name, config, prefab)
        {
            rootCG.alpha = 0f;
            artAssetsDirectory = rootCharacterFolder + "/Images";
            
            GetLayers();
        }

        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(SPRITE_RENDERER_PARENT_NAME);
            
            if (rendererRoot == null)
                return;

            for (int i  = 0; i  < rendererRoot.transform.childCount; i ++)
            {
                Transform child = rendererRoot.transform.GetChild(i);
                Image image = child.GetComponent<Image>();

                if (image != null)
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(image, i);
                    spriteLayers.Add(layer);
                    child.name = $"Layer: {i}";
                }
            }
        }

        public void SetSprite(Sprite sprite, int layer = 0)
        {
            spriteLayers[layer].SetSprite(sprite);
        }

        public Sprite GetSprite(string spriteName, string textureName = "")
        {
            if (config.characterType == CharacterType.SpriteSheet)
            {
                string path = string.IsNullOrEmpty(textureName) ? SPRITE_DEFAULT_TEXTURE_NAME : textureName;
                Sprite[] sprites = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{path}");
                
                if (sprites.Length == 0)
                    Debug.LogError($"Character {name} has no SpriteSheet at path {path}");
                
                return Array.Find(sprites, s => s.name == spriteName);
            }
            else
            {
                return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");
            }
        }
        
        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0f;
            CanvasGroup self = rootCG;

            while (!ZlipUtilities.ApproximatelyWithMargin(self.alpha, targetAlpha, 0.001f))
            {
                self.alpha = Mathf.Lerp(self.alpha, targetAlpha, showHideSpeed * Time.deltaTime);
                yield return null;
            }

            co_Revealing = null;
            co_Hiding = null;
        }
    }
}