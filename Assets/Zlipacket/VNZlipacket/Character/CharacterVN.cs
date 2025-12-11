using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Zlipacket.VNZlipacket.Dialogue;

namespace Zlipacket.VNZlipacket.Character
{
    public abstract class CharacterVN
    {
        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterVNConfigData config;
        public Animator animator;

        protected CharacterVNManager manager => CharacterVNManager.Instance;
        public DialogueSystem dialogueSystem => DialogueSystem.Instance;

        //Coroutine
        protected Coroutine co_Revealing, co_Hiding;
        protected Coroutine co_Moving;
        public bool isRevealing => co_Revealing != null;
        public bool isHiding => co_Hiding != null;
        public bool isMoving => co_Moving != null;
        public virtual bool isVisible => false;

        public CharacterVN(string name, CharacterVNConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject ob = GameObject.Instantiate(prefab, manager.characterPanel);
                ob.name = manager.FormatCharacterPath(manager.characterPrefabNameFormat, name);
                ob.SetActive(true);
                root = ob.GetComponent<RectTransform>();
                animator = root.GetComponentInChildren<Animator>();
            }
        }

        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet,
            Live2D,
            Model3D
        }

        public Coroutine Say(string dialogue) => Say(new List<string>() { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            UpdateTextCustomizationOnScreen();
            return dialogueSystem.Say(dialogue);
        }

        public void SetNameColor(Color color) => config.nameColor = color;
        public void SetDialogueColor(Color color) => config.dialogueColor = color;
        public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
        public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;

        public void ResetConfigData() => config = CharacterVNManager.Instance.GetCharacterConfig(name);

        public void UpdateTextCustomizationOnScreen() =>
            dialogueSystem.ApplySpeakerDataToDialogueContainer(config);

        public virtual Coroutine Show()
        {
            if (isRevealing)
                return co_Revealing;

            if (isHiding)
                manager.StopCoroutine(co_Hiding);

            co_Revealing = manager.StartCoroutine(ShowingOrHiding(true));
            return co_Revealing;
        }

        public virtual Coroutine Hide()
        {
            if (isHiding)
                return co_Hiding;

            if (isRevealing)
                manager.StopCoroutine(co_Revealing);

            co_Hiding = manager.StartCoroutine(ShowingOrHiding(false));
            return co_Hiding;
        }

        public virtual IEnumerator ShowingOrHiding(bool show)
        {
            Debug.LogError("Show/Hide can not be called from the base class.");
            yield return null;
        }
        
        /// <summary>
        /// Set position of character from 0 to 1 for left to right and bottom to top in the constraint of anchorpoint of character.
        /// If out of range, it will be out of screen boundary.
        /// </summary>
        /// <param name="position"></param>
        public virtual void SetPosition2D(Vector2 position)
        {
            if (root == null)
                return;
            
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUIPositionToRelativeAnchorTarget(position);
            
            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }
        
        public virtual Coroutine MoveToPosition2D(Vector2 position, float speed = 2f, bool smooth = false)
        {
            if (root == null)
                return null;
            
            if (isMoving)
                manager.StopCoroutine(co_Moving);
            
            co_Moving = manager.StartCoroutine(MovingToPosition2D(position, speed, smooth));
            return co_Moving;
        }

        private IEnumerator MovingToPosition2D(Vector2 position, float speed, bool smooth = false)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUIPositionToRelativeAnchorTarget(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            while (root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
            {
                root.anchorMin = smooth ? 
                    Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime) :
                    Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

                root.anchorMax = root.anchorMin + padding;

                if (smooth && Vector2.Distance(root.anchorMin, minAnchorTarget) <= 0.001f)
                {
                    root.anchorMin = minAnchorTarget;
                    root.anchorMax = maxAnchorTarget;
                    break;
                }
                
                yield return null;
            }
            
            co_Moving = null;
        }

        protected (Vector2, Vector2) ConvertUIPositionToRelativeAnchorTarget(Vector2 position)
        {
            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;
            
            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;
            
            return (minAnchorTarget, maxAnchorTarget);
        }
    }
}