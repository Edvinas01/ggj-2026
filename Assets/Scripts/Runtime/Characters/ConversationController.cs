using System;
using System.Linq;
using InSun.JamOne.Core.Utilities;
using RIEVES.GGJ2026.Runtime.Resources;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private ConversationViewController viewController;

        [SerializeField]
        private ResourceController resourceController;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onCorrectChoiceSelected;

        [SerializeField]
        private UnityEvent onIncorrectChoiceSelected;

        [SerializeField]
        private UnityEvent onRandomBlurbChoiceSelected;

        public CharacterActor conversingWith { get; private set; }

        public event Action OnConversationStarted;

        public event Action OnConversationStopped;

        private void OnEnable()
        {
            viewController.OnChoiceSelected += OnChoiceSelected;
        }

        private void OnDisable()
        {
            viewController.OnChoiceSelected -= OnChoiceSelected;
        }

        public void StartConversation(CharacterActor character)
        {
            conversingWith = character;

            var conversationData = character.CharacterData.Conversation;
            if (conversationData.ConversedCount <= 0)
            {
                var correctIncorrect = conversationData.Messages.Where(m => m.MessageType == CharacterMessageType.CorrectIncorrect);
                if (correctIncorrect.TryGetRandom(out var message))
                {
                    OnCorrectIncorrectConversation(character, message);

                    conversationData.ConversedCount++;

                    viewController.ShowView();
                    OnConversationStarted?.Invoke();
                    return;
                }
            }

            var randomBlurbs = conversationData.Messages.Where(m => m.MessageType == CharacterMessageType.RandomBlurb);
            if (randomBlurbs.TryGetRandom(out var blurb))
            {
                OnRandomBlurbChoiceConversation(character, blurb);

                conversationData.ConversedCount++;

                viewController.ShowView();
                OnConversationStarted?.Invoke();
                return;
            }

            StopConversation();
        }

        public void StopConversation()
        {
            viewController.HideView();

            conversingWith?.ConversationStopped();
            conversingWith = null;

            OnConversationStopped?.Invoke();
        }

        private void OnCorrectIncorrectConversation(CharacterActor character, CharacterMessageData message)
        {
            var correctChoices = message.CorrectChoices
                .OrderBy(_ => Random.value)
                .Select(choice => new ConversationChoice(
                        isCorrect: true,
                        content: choice,
                        messageType: message.MessageType
                    )
                )
                .Take(1);

            var incorrectChoices = message.IncorrectChoices
                .OrderBy(_ => Random.value)
                .Select(choice => new ConversationChoice(
                        isCorrect: false,
                        content: choice,
                        messageType: message.MessageType
                    )
                )
                .Take(3);

            var choices = correctChoices
                .Concat(incorrectChoices)
                .OrderBy(_ => Random.value)
                .ToList();

            viewController.Initialize(
                title: character.CharacterData.CharacterName,
                content: message.Content,
                choices: choices
            );
        }

        private void OnRandomBlurbChoiceConversation(CharacterActor character, CharacterMessageData message)
        {
            viewController.Initialize(
                title: character.CharacterData.CharacterName,
                content: message.Content,
                choices: new[]
                {
                    new ConversationChoice(
                        messageType: CharacterMessageType.RandomBlurb,
                        isCorrect: true,
                        content: null
                    ),
                }
            );
        }

        private void OnChoiceSelected(ConversationChoice choice)
        {
            switch (choice.MessageType)
            {
                case CharacterMessageType.CorrectIncorrect:
                {
                    if (choice.IsCorrect)
                    {
                        resourceController.AddAlcohol(conversingWith.CharacterData.AddsAlcohol);
                        onCorrectChoiceSelected.Invoke();
                    }
                    else
                    {
                        resourceController.UseAlcohol(conversingWith.CharacterData.RemovesAlcohol);
                        onIncorrectChoiceSelected.Invoke();
                    }

                    break;
                }
                case CharacterMessageType.RandomBlurb:
                {
                    onRandomBlurbChoiceSelected.Invoke();
                    break;
                }
            }

            StopConversation();
        }
    }
}
