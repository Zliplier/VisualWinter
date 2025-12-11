using UnityEngine;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.Audio
{
    public class SoundFXManager : Singleton<SoundFXManager>
    {
        [SerializeField] private AudioSource soundFXObject;
        
        public void PlaySoundFX(AudioClip clip, Transform spawnTransform, float volume = 1f)
        {
            AudioSource soundFX = Instantiate(soundFXObject, spawnTransform);
            
            soundFX.clip = clip;
            soundFX.volume = volume;
            soundFX.Play();
            
            float clipLength = clip.length;
            Destroy(soundFX.gameObject, clipLength);
        }
    }
}