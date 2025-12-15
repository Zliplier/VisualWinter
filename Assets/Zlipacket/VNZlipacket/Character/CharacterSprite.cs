using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.VNZlipacket.Character
{
    public class CharacterSprite : VN_Character
    {
        private const string SPRITE_RENDERER_PARENT_NAME = "Renderers";
        private const string SPRITE_DEFAULT_TEXTURE_NAME = "Default";
        
        public float showHideSpeed = 3f;
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();
        
        public List<CharacterSpriteLayer> spriteLayers = new List<CharacterSpriteLayer>();

        private string artAssetsDirectory = "";

        public override bool isVisible
        {
            get { return isRevealing || ZlipUtilities.ApproximatelyWithMargin(rootCG.alpha, 1, 0.001f); }
            set { rootCG.alpha = value ? 1 : 0; }
        }
        
        public CharacterSprite(string name, CharacterConfigData config, GameObject prefab, string rootCharacterFolder) : base(name, config, prefab)
        {
            rootCG.alpha = ENABLE_ON_START ? 1 : 0;
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
                Image image = child.GetComponentInChildren<Image>();

                if (image != null)
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(image, i);
                    spriteLayers.Add(layer);
                    child.name = $"Sprite Layer: {i}";
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

        public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 1f)
        {
            CharacterSpriteLayer spriteLayer = spriteLayers[layer];
            
            return spriteLayer.TransitionSprite(sprite, speed);
        }
        
        public override IEnumerator ShowingOrHiding(bool show, float speedMultiplyer = 1f)
        {
            float targetAlpha = show ? 1f : 0f;
            CanvasGroup self = rootCG;

            while (!ZlipUtilities.ApproximatelyWithMargin(self.alpha, targetAlpha, 0.001f))
            {
                self.alpha = Mathf.Lerp(self.alpha, targetAlpha, showHideSpeed * Time.deltaTime * speedMultiplyer);
                yield return null;
            }

            co_Revealing = null;
            co_Hiding = null;
        }

        public override void SetColor(Color color)
        {
            base.SetColor(color);

            color = displayColor;

            foreach (CharacterSpriteLayer layer in spriteLayers)
            {
                layer.StopChangingColor();
                layer.SetColor(color);
            }
        }

        public override IEnumerator ChangingColor(Color color, float speed)
        {
            foreach (CharacterSpriteLayer layer in spriteLayers)
                layer.TransitionColor(color, speed);
            
            yield return null;

            while (spriteLayers.Any(l => l.isChangingColor))
                yield return null;
            
            co_ChangingColor = null;
        }

        protected override IEnumerator HighLighting(bool highlight, float speedMultiplyer = 1f, bool immediate = false)
        {
            Color targetColor = displayColor;

            foreach (CharacterSpriteLayer layer in spriteLayers)
            {
                if (!immediate)
                    layer.TransitionColor(targetColor, speedMultiplyer);
                else
                    layer.SetColor(targetColor);
            }
            
            yield return null;

            while (spriteLayers.Any(l => l.isTransitioning))
                yield return null;

            co_Highlighting = null;
        }

        public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplyer, bool immediate = false)
        {
            foreach (CharacterSpriteLayer layer in spriteLayers)
            {
                if (faceLeft)
                    layer.FaceLeft(speedMultiplyer, immediate);
                else
                    layer.FaceRight(speedMultiplyer, immediate);
            }
            
            yield return null;
            
            while (spriteLayers.Any(l => l.isTransitioning))
                yield return null;
            
            co_Flipping = null;
        }

        public override IEnumerator ChangingExpression(int layer, string expression, bool immediate = false)
        {
            Sprite sprite = GetSprite(expression);

            if (sprite == null)
            {
                Debug.LogError($"Character {name} has no Sprite of {expression}");
                yield break;
            }
            
            if (!immediate)
                TransitionSprite(sprite, layer);
            else
            {
                SetSprite(sprite, layer);
                yield break;
            }
            
            while (spriteLayers.Any(l => l.isTransitioning))
                yield return null;
            
            co_ChangingExpression = null;
        }
    }
}