using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Zlipacket.VNZlipacket.Dialogue;

namespace Zlipacket.VNZlipacket.Character
{
    public abstract class VN_Character
    {
        public const bool ENABLE_ON_START = false;
        private const float UNHEIGHTLIGHTED_DARKEN_STRENGTH = 0.65f;
        public const bool DEFAULT_ORIENTATION_LEFT = true;
        public const string ANIMATION_REFRESH_TRIGGER = "Refresh";
        
        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfigData config;
        public Animator animator;
        public Color color { get; protected set; } = Color.white;
        protected Color displayColor => isHighlighted ? highlightedColor : unHighlightColor;
        protected Color highlightedColor => color;
        protected Color unHighlightColor => new Color(
            color.r * UNHEIGHTLIGHTED_DARKEN_STRENGTH, 
            color.g * UNHEIGHTLIGHTED_DARKEN_STRENGTH, 
            color.b * UNHEIGHTLIGHTED_DARKEN_STRENGTH, color.a);
        public bool isHighlighted { get; protected set; } = true;
        protected bool facingLeft = DEFAULT_ORIENTATION_LEFT;
        public int priority { get; protected set; }
        
        protected VN_CharacterManager characterManager => VN_CharacterManager.Instance;
        public DialogueSystem dialogueSystem => DialogueSystem.Instance;

        //Coroutine
        protected Coroutine co_Revealing, co_Hiding;
        protected Coroutine co_Moving;
        protected Coroutine co_ChangingColor;
        protected Coroutine co_Highlighting;
        protected Coroutine co_Flipping;
        protected Coroutine co_ChangingExpression;
        
        public bool isRevealing => co_Revealing != null;
        public bool isHiding => co_Hiding != null;
        public bool isMoving => co_Moving != null;
        public bool isChangingColor => co_ChangingColor != null;
        public bool isHighlighting => (isHighlighted && co_Highlighting != null);
        public bool isUnHighlighting => (!isHighlighted && co_Highlighting != null);
        public bool isFacingLeft => facingLeft;
        public bool isFacingRight => !facingLeft;   
        public bool isFlipping => co_Flipping != null;
        public bool isChangingExpression => co_ChangingExpression != null;
        
        public virtual bool isVisible { get; set; }

        public VN_Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject ob = GameObject.Instantiate(prefab, characterManager.characterPanel);
                ob.name = characterManager.FormatCharacterPath(characterManager.characterPrefabNameFormat, name);
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

        public void ResetConfigData() => config = VN_CharacterManager.Instance.GetCharacterConfig(name);

        public void UpdateTextCustomizationOnScreen() =>
            dialogueSystem.ApplySpeakerDataToDialogueContainer(config);

        public virtual Coroutine Show(float speedMultiplyer = 1f)
        {
            if (isRevealing)
                return co_Revealing;

            if (isHiding)
                characterManager.StopCoroutine(co_Hiding);

            co_Revealing = characterManager.StartCoroutine(ShowingOrHiding(true));
            return co_Revealing;
        }

        public virtual Coroutine Hide(float speedMultiplyer = 1f)
        {
            if (isHiding)
                return co_Hiding;

            if (isRevealing)
                characterManager.StopCoroutine(co_Revealing);

            co_Hiding = characterManager.StartCoroutine(ShowingOrHiding(false));
            return co_Hiding;
        }

        public virtual IEnumerator ShowingOrHiding(bool show, float speedMultiplyer = 1f)
        {
            Debug.LogError("Show/Hide can not be called from the base class.");
            yield return null;
        }
        
        /// <summary>
        /// Set position of character from 0 to 1 for left to right and bottom to top in the constraint of anchorpoint of character.
        /// If out of range, it will be out of screen boundary.
        /// </summary>
        /// <param name="position"></param>
        public virtual void SetPosition(Vector2 position)
        {
            if (root == null)
                return;
            
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUIPositionToRelativeAnchorTarget(position);
            
            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }
        
        public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2f, bool smooth = false)
        {
            if (root == null)
                return null;
            
            if (isMoving)
                characterManager.StopCoroutine(co_Moving);
            
            co_Moving = characterManager.StartCoroutine(MovingToPosition2D(position, speed, smooth));
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

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        public Coroutine TransitionToColor(Color color, float speed = 1f)
        {
            this.color = color;
            
            if (isChangingColor)
                characterManager.StopCoroutine(co_ChangingColor);

            co_ChangingColor = characterManager.StartCoroutine(ChangingColor(displayColor, speed));
            return co_ChangingColor;
        }

        public virtual IEnumerator ChangingColor(Color color, float speed)
        {
            Debug.LogError("Set Color can not be called from the base class.");
            yield return null;
        }

        public Coroutine Highlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting)
                return co_Highlighting;
            
            if (isUnHighlighting)
                characterManager.StopCoroutine(co_Highlighting);

            isHighlighted = true;

            co_Highlighting = characterManager.StartCoroutine(HighLighting(isHighlighted, speed, immediate));
            return co_Highlighting;
        }

        public Coroutine UnHightlight(float speed = 1f, bool immediate = false)
        {
            if (isUnHighlighting)
                return co_Highlighting;
            
            if (isUnHighlighting)
                characterManager.StopCoroutine(co_Highlighting);

            isHighlighted = false;
                
            co_Highlighting = characterManager.StartCoroutine(HighLighting(isHighlighted, speed, immediate));
            return co_Highlighting;
        }
        
        protected virtual IEnumerator HighLighting(bool highlight, float speedMultiplyer = 1f, bool immediate = false)
        {
            Debug.LogError("HighLighting can not be called from the base class.");
            yield return null;
        }
        
        public Coroutine Flip(float speed = 1f, bool immediate = false)
        {
            if (isFacingLeft)
                return FaceRight(speed, immediate);
            else
                return FaceLeft(speed, immediate);
        }

        public Coroutine FaceLeft(float speed = 1f, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_Flipping);

            facingLeft = true;
            co_Flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
            
            return co_Flipping;
        }

        public Coroutine FaceRight(float speed = 1f, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_Flipping);

            facingLeft = false;
            co_Flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
            
            return co_Flipping;
        }

        public virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplyer, bool immediate = false)
        {
            Debug.LogError("Flipping can not be called from the base class.");
            yield return null;
        }

        public void SetPriority(int priority, bool autoSortCharactersOnUI = true)
        {
            this.priority = priority;
            
            if (autoSortCharactersOnUI)
            {
                characterManager.SortCharacters();
            }
        }

        public void Animate(string animationName)
        {
            animator.SetTrigger(animationName);
        }
        
        public void Animate(string animationName, bool state)
        {
            animator.SetBool(animationName, state);
            animator.SetTrigger(ANIMATION_REFRESH_TRIGGER);
        }

        public virtual void OnSort(int sortingIndex)
        {
            return;
        }
        
        public virtual Coroutine OnRecieveCastingExpression(int layer, string expression, bool immediate = false)
        {
            if (isChangingExpression)
                characterManager.StopCoroutine(co_ChangingExpression);
            
            co_ChangingExpression = characterManager.StartCoroutine(ChangingExpression(layer, expression, immediate));
            return co_ChangingExpression;
        }

        public virtual IEnumerator ChangingExpression(int layer, string expression, bool immediate = false)
        {
            co_ChangingExpression = null;
            yield break;
        }
    }
}