using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Zlipacket.VNZlipacket.Character
{
    public class CharacterSpriteLayer
    {
        private VN_CharacterManager characterManager => VN_CharacterManager.Instance;

        private const float DEFAULT_TRANSITION_SPEED = 3f;
        private float transitionSpeedMultiplyer = 1f;
        
        public int layer { get; private set; } = 0;
        public Image renderer { get; private set; } = null;
        public CanvasGroup rendererCG => renderer.GetComponent<CanvasGroup>();
        
        private List<CanvasGroup> oldRenderers = new List<CanvasGroup>();

        private Coroutine co_TransitioningLayer = null;
        private Coroutine co_LevelingAlpha = null;
        private Coroutine co_ChangingColor = null;
        private Coroutine co_Flipping = null;

        private bool isFacingLeft = VN_Character.DEFAULT_ORIENTATION_LEFT;
        
        public bool isTransitioning => co_TransitioningLayer != null;
        public bool isLevelingAlpha => co_LevelingAlpha != null;
        public bool isChangingColor => co_ChangingColor != null;
        public bool isFlipping => co_Flipping != null;

        public CharacterSpriteLayer(Image defaultRenderer, int layer = 0)
        {
            renderer = defaultRenderer;
            this.layer = layer;
        }

        public void SetSprite(Sprite sprite)
        {
            renderer.sprite = sprite;
        }

        public Coroutine TransitionSprite(Sprite sprite, float speed = 1f)
        {
            if (sprite == renderer.sprite)
                return null;

            if (isTransitioning)
                characterManager.StopCoroutine(co_TransitioningLayer);

            co_TransitioningLayer = characterManager.StartCoroutine(TransitioningSprite(sprite, speed));
            
            return co_TransitioningLayer;
        }

        public IEnumerator TransitioningSprite(Sprite sprite, float speedMultiplyer)
        {
            transitionSpeedMultiplyer = speedMultiplyer;
            
            Image newRenderer = CreateRenderer(renderer.transform.parent);
            newRenderer.sprite = sprite;

            yield return TryStartLevelingAlpha();
            co_TransitioningLayer = null;
        }

        private Image CreateRenderer(Transform parent)
        {
            Image newRenderer = Object.Instantiate(renderer, parent);
            oldRenderers.Add(rendererCG);
            
            newRenderer.name = renderer.name;
            renderer = newRenderer;
            renderer.gameObject.SetActive(true);
            rendererCG.alpha = 0;
            
            return newRenderer;
        }

        private Coroutine TryStartLevelingAlpha()
        {
            if (isLevelingAlpha)
                return co_LevelingAlpha;

            co_LevelingAlpha = characterManager.StartCoroutine(RunAlphaLeveling());
            return co_LevelingAlpha;
        }

        private IEnumerator RunAlphaLeveling()
        {
            float speed = DEFAULT_TRANSITION_SPEED * transitionSpeedMultiplyer * Time.deltaTime;
            
            while (rendererCG.alpha < 1f || oldRenderers.Any(oldCG => oldCG.alpha > 0))
            {
                rendererCG.alpha = Mathf.MoveTowards(rendererCG.alpha, 1f, speed);
                
                for (int i = oldRenderers.Count - 1; i >= 0; i--)
                {
                    CanvasGroup oldCG = oldRenderers[i];
                    oldCG.alpha = Mathf.MoveTowards(oldCG.alpha, 0f, speed);

                    if (oldCG.alpha <= 0f)
                    {
                        oldRenderers.RemoveAt(i);
                        Object.Destroy(oldCG.gameObject);
                    }
                }

                yield return null;
            }

            co_LevelingAlpha = null;
        }

        public void SetColor(Color color)
        {
            renderer.color = color;

            foreach (CanvasGroup oldCG in oldRenderers)
            {
                oldCG.GetComponent<Image>().color = color;
            }
        }

        public Coroutine TransitionColor(Color color, float speed)
        {
            if (isChangingColor)
                characterManager.StopCoroutine(co_ChangingColor);

            co_ChangingColor = characterManager.StartCoroutine(ChangingColor(color, speed));
            return co_ChangingColor;
        }
        
        public void StopChangingColor()
        {
            if (!isChangingColor)
                return;
            
            characterManager.StopCoroutine(co_ChangingColor);
            
            co_ChangingColor = null;
        }

        private IEnumerator ChangingColor(Color color, float speed)
        {
            Color oldColor = renderer.color;
            List<Image> oldImages = new();

            foreach (CanvasGroup oldCG in oldRenderers)
            {
                oldImages.Add(oldCG.GetComponent<Image>());
            }

            float colorPercent = 0f;

            while (colorPercent < 1f)
            {
                colorPercent += Time.deltaTime * DEFAULT_TRANSITION_SPEED * transitionSpeedMultiplyer;
                
                renderer.color = Color.Lerp(oldColor, color, colorPercent);
                
                foreach (Image oldImage in oldImages)
                {
                    oldImage.color = renderer.color;
                }
                
                yield return null;
            }
            
            co_ChangingColor = null;
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

            isFacingLeft = true;

            co_Flipping = characterManager.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));
            return co_Flipping;
        }
        
        public Coroutine FaceRight(float speed = 1f, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_Flipping);

            isFacingLeft = false;

            co_Flipping = characterManager.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));
            return co_Flipping;
        }

        private IEnumerator FaceDirection(bool faceLeft, float speedMultiplyer, bool immediate)
        {
            float xScale = faceLeft ? 1f : -1f;
            Vector3 newScale = new Vector3(xScale, 1f, 1f);
            
            if (!immediate)
            {
                Image newRenderer = CreateRenderer(renderer.transform.parent);
                newRenderer.transform.localScale = newScale;
                
                transitionSpeedMultiplyer = speedMultiplyer;
                TryStartLevelingAlpha();
                
                while (isLevelingAlpha)
                    yield return null;
            }
            else
            {
                renderer.transform.localScale = newScale;
            }

            co_Flipping = null;
        }
    }
}