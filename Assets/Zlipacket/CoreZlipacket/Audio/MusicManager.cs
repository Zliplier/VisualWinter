using System.Collections.Generic;
using UnityEngine;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.Audio
{
    public class MusicManager : Singleton<MusicManager>
    {
        [SerializeField] private AudioSource musicObject;

        private Dictionary<string, AudioSource> musicPlaylist = new();

        public void PlayMusic(AudioClip clip, float volume = 1f)
        {
            AudioSource music = Instantiate(musicObject, Instance.transform);
            
            music.clip = clip;
            music.volume = volume;
            music.Play();
        }
        
        public void PlayMusicWithCallback(AudioClip clip, string callbackName, Transform spawnTransform, float volume = 1f)
        {
            AudioSource music = Instantiate(musicObject, Instance.transform, spawnTransform);
            
            music.clip = clip;
            music.volume = volume;
            music.Play();
            
            musicPlaylist.Add(callbackName, music);
        }

        public void StopMusicByCallback(string callbackName)
        {
            if (musicPlaylist.TryGetValue(callbackName, out AudioSource music))
            {
                Destroy(music.gameObject);
            }
            else
            {
                Debug.LogWarning(callbackName + " not found in Music Stack.");
            }
        }

        public void StopAllMusic()
        {
            foreach (var music in musicPlaylist.Values)
            {
                music.Stop();
            }
            musicPlaylist.Clear();
        }

        public bool CheckIsSongPlaying(string callbackName)
        {
            if (musicPlaylist.TryGetValue(callbackName, out AudioSource music))
            {
                return music.isPlaying;
            }
            return false;
        }
    }
}