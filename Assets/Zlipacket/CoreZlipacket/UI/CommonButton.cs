using UnityEngine;
using UnityEngine.EventSystems;
using Zlipacket.CoreZlipacket.Audio;
using Zlipacket.CoreZlipacket.Scene;

namespace Zlipacket.CoreZlipacket.UI
{
    public class CommonButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] public Texture2D curserSprite;
        [SerializeField] public Texture2D hoverSprite;
        [SerializeField] public AudioClip clickSound;

        public void PlayClickSound()
        {
            if (clickSound != null)
            {
                SoundFXManager.Instance.PlaySoundFX(clickSound, transform);
            }
        }
        
        public void ChangeToScene(string sceneName)
        {
            SceneController.Instance.LoadScene(sceneName);
        }
        
        public void Quit()
        {
            Application.Quit();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverSprite == null) return;
            Vector2 cursorHotSpot = new Vector2(hoverSprite.width / 2, hoverSprite.height / 2);
            Cursor.SetCursor(hoverSprite, cursorHotSpot, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (curserSprite == null) return;
            Vector2 cursorHotSpot = new Vector2(curserSprite.width / 2, curserSprite.height / 2);
            Cursor.SetCursor(curserSprite, cursorHotSpot, CursorMode.Auto);
        }
    }
}