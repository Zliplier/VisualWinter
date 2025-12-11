using System;
using System.Collections;
using UnityEngine;
using Zlipacket.VNZlipacket.Character;

namespace TESTING
{
    public class TestCharacter : MonoBehaviour
    {
        private CharacterVN CreateCharacter(string name) => CharacterVNManager.Instance.CreateCharacter(name); 
        
        private void Start()
        {
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            CharacterVN MobCharacter1 = CreateCharacter("MobCharacter1 as Haru");
            CharacterVN MobCharacter2 = CreateCharacter("MobCharacter2 as Haru");
            CharacterVN MobCharacter3 = CreateCharacter("MobCharacter3 as Haru");
            
            MobCharacter1.SetPosition2D(new Vector2(0.5f, 0f));
            MobCharacter2.SetPosition2D(new Vector2(0.5f, 0f));
            MobCharacter3.SetPosition2D(new Vector2(0.5f, 0f));
            
            yield return MobCharacter1.Show();
            yield return MobCharacter1.MoveToPosition2D(new Vector2(0f, 0f), smooth: true);
            
            yield return MobCharacter2.Show();
            yield return MobCharacter2.MoveToPosition2D(new Vector2(1f, 0f), smooth: true);
            
            yield return MobCharacter3.Show();
        }
    }
}