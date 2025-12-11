using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zlipacket.CoreZlipacket.Player.Input;
using Zlipacket.CoreZlipacket.Tools;
using Zlipacket.VNZlipacket.Character;
using Zlipacket.VNZlipacket.Dialogue.DialogueData;
using Zlipacket.VNZlipacket.ScriptableObjects;

namespace Zlipacket.VNZlipacket.Dialogue
{
    public class DialogueSystem : Singleton<DialogueSystem>
    {
        [SerializeField] private SO_DialogueSystemConfig _config;
        public SO_DialogueSystemConfig config => _config;
        
        public DialogueContainer dialogueContainer = new();
        public ConversationManager conversationManager;
        
        private TextArchitect architect;
        public bool isRunningConversation => conversationManager.isRunning;
        
        public UnityEvent onUserPromptNext;
        
        public override void Initialize()
        {
            base.Initialize();
            
            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);
        }

        private void Start()
        {
            PlayerInputController.Instance?.onJump.AddListener(OnUserPromptNext);
            PlayerInputController.Instance?.onEnter.AddListener(OnUserPromptNext);
        }

        private void OnEnable()
        {
            PlayerInputController.Instance?.onJump.AddListener(OnUserPromptNext);
            PlayerInputController.Instance?.onEnter.AddListener(OnUserPromptNext);
        }
        
        private void OnDisable()
        {
            PlayerInputController.Instance?.onJump.RemoveListener(OnUserPromptNext);
            PlayerInputController.Instance?.onEnter.RemoveListener(OnUserPromptNext);
        }

        public void OnUserPromptNext()
        {
            onUserPromptNext?.Invoke();
        }

        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            CharacterVN character = CharacterVNManager.Instance.GetCharacter(speakerName);
            CharacterVNConfigData config = character != null ? character.config : CharacterVNManager.Instance.GetCharacterConfig(speakerName);
            
            ApplySpeakerDataToDialogueContainer(config);
        }
        
        public void ApplySpeakerDataToDialogueContainer(CharacterVNConfigData config)
        {
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            dialogueContainer.NameContainer.SetNameColor(config.nameColor);
            dialogueContainer.NameContainer.SetNameFont(config.nameFont);
        }
        
        public void ShowSpeakerName(string speakerName)
        {
            if (speakerName.ToLower() != "narrator")
                dialogueContainer.NameContainer.Show(speakerName);
            else
                HideSpeakerName();
        }

        public void HideSpeakerName()
        {
            dialogueContainer.NameContainer.Hide();
        }

        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            return Say(conversation);
        }

        public Coroutine Say(List<string> conversation)
        {
            return conversationManager.StartConversation(conversation);
        }
    }
}