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
            if (conversationData.Messages.TryGetRandom(out var randomMessage) == false)
            {
                return;
            }

            var correctChoices = randomMessage.CorrectChoices
                .OrderBy(_ => Random.value)
                .Select(choice => new ConversationChoice(
                        isCorrect: true,
                        content: choice
                    )
                )
                .Take(1);

            var incorrectChoices = randomMessage.IncorrectChoices
                .OrderBy(_ => Random.value)
                .Select(choice => new ConversationChoice(
                        isCorrect: false,
                        content: choice
                    )
                )
                .Take(3);

            var choices = correctChoices
                .Concat(incorrectChoices)
                .OrderBy(_ => Random.value)
                .ToList();

            viewController.Initialize(
                title: character.CharacterData.CharacterName,
                content: randomMessage.Content,
                choices: choices
            );

            viewController.ShowView();

            OnConversationStarted?.Invoke();
        }

        public void StopConversation()
        {
            viewController.HideView();

            conversingWith?.ConversationStopped();
            conversingWith = null;

            OnConversationStopped?.Invoke();
        }

        private void OnChoiceSelected(ConversationChoice choice)
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

            viewController.HideView();

            conversingWith?.ConversationStopped();
            conversingWith = null;

            OnConversationStopped?.Invoke();
        }
    }
}
