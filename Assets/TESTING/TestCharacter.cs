using System;
using System.Collections;
using UnityEngine;
using Zlipacket.VNZlipacket.Character;

namespace TESTING
{
    public class TestCharacter : MonoBehaviour
    {
        private VN_Character CreateCharacter(string name) => VN_CharacterManager.Instance.CreateCharacter(name); 
        
        private void Start()
        {
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            CharacterSprite MobCharacter1 = CreateCharacter("MobCharacter1 as Haru") as CharacterSprite;
            CharacterSprite MobCharacter2 = CreateCharacter("MobCharacter2 as Haru") as CharacterSprite;

            MobCharacter1.SetPosition(new Vector2(0.5f, 0f));
            MobCharacter2.SetPosition(new Vector2(0.5f, 0f));

            MobCharacter1.Show();
            yield return MobCharacter2.Show();

            MobCharacter1.Flip();

            yield return new WaitForSeconds(1f);
            
            MobCharacter1.SetPriority(1);
        }
    }
}