using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zlipacket.VNZlipacket.Character
{
    public class CharacterSpriteLayer
    {
        private CharacterVNManager CharacterManager => CharacterVNManager.Instance;
        
        public int layer { get; private set; } = 0;
        public Image renderer { get; private set; } = null;
        public CanvasGroup rendererCG => renderer.GetComponent<CanvasGroup>();
        
        private List<CanvasGroup> oldRenderers = new List<CanvasGroup>();

        public CharacterSpriteLayer(Image defaultRenderer, int layer = 0)
        {
            renderer = defaultRenderer;
            this.layer = layer;
        }

        public void SetSprite(Sprite sprite)
        {
            
        }
    }
}