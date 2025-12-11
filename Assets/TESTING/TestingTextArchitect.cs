using System;
using UnityEngine;
using Zlipacket.VNZlipacket.Dialogue;
using Random = UnityEngine.Random;

namespace TESTING
{
    public class TestingTextArchitect : MonoBehaviour
    {
        DialogueSystem dialogueSystem;
        TextArchitect textArchitect;
        public TextArchitect.BuildMethod buildMethod = TextArchitect.BuildMethod.Instant;

        private string[] testingDialogue = new string[4]
        {
            "Test Dialogue 101.", 
            "The crow is black, it is not apple.", 
            "Hey you! You're finally awake.", 
            "Oi mate! Wat'ch where yer goin'."
        };
        
        
        private void Start()
        {
            dialogueSystem = DialogueSystem.Instance;
            textArchitect = new TextArchitect(dialogueSystem.dialogueContainer.dialogueText);
            textArchitect.buildMethod = buildMethod;
            textArchitect.speed = 0.5f;
        }

        private void Update()
        {
            string longline =
                "The hypothesis of space time base on the law of gravitational relativity states that once something goes over the speed of sound, it will affect space time continuum and every one will die.";

            if (buildMethod != textArchitect.buildMethod)
            {
                textArchitect.buildMethod = buildMethod;
                textArchitect.Stop();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                textArchitect.Stop();
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (textArchitect.isBuilding)
                {
                    if (!textArchitect.hurryUp)
                    {
                        textArchitect.hurryUp = true;
                    }
                    else
                    {
                        textArchitect.ForceComplete();
                    }
                }
                else
                {
                    //textArchitect.Build(longline);
                    textArchitect.Build(testingDialogue[Random.Range(0, testingDialogue.Length)]);
                }
            } else if (Input.GetKeyDown(KeyCode.R))
            {
                textArchitect.Append(testingDialogue[Random.Range(0, testingDialogue.Length)]);
            }
        }
    }
}