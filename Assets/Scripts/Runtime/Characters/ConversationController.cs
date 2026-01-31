using System;
using System.Linq;
using CHARK.GameManagement;
using InSun.JamOne.Core.Utilities;
using RIEVES.GGJ2026.Runtime.Heat;
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

        [Header("Features")]
        [Min(1)]
        [SerializeField]
        private int maxMessagesPerConvo = 3;

        [Min(0)]
        [SerializeField]
        private int maxCorrectMessages = 1;

        [Min(0)]
        [SerializeField]
        private int maxIncorrectMessages = 3;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onCorrectChoiceSelected;

        [SerializeField]
        private UnityEvent onIncorrectChoiceSelected;

        [SerializeField]
        private UnityEvent onRandomBlurbChoiceSelected;

        private HeatSystem heatSystem;

        private int currentMessageCount;
        private int currentMessageMax;

        public CharacterActor conversingWith { get; private set; }

        public event Action OnConversationStarted;

        public event Action OnConversationStopped;

        private void Awake()
        {
            heatSystem = GameManager.GetSystem<HeatSystem>();
        }

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
            currentMessageCount = 0;

            currentMessageMax = Random.Range(1, (int)(maxMessagesPerConvo * heatSystem.CurrentHeat));

            Converse(character);
        }

        public void StopConversation()
        {
            viewController.HideView();

            if (conversingWith)
            {
                conversingWith.CharacterData.ConversationData.ConversedCount++;
                conversingWith.ConversationStopped();
                conversingWith = null;
            }

            currentMessageCount = 0;

            OnConversationStopped?.Invoke();
        }

        private void Converse(CharacterActor character)
        {
            var conversationData = character.CharacterData.ConversationData;
            if (conversationData.ConversedCount <= 0)
            {
                var correctIncorrect = conversationData.Messages.Where(m => m.MessageType == CharacterMessageType.CorrectIncorrect);
                if (correctIncorrect.TryGetRandom(out var message))
                {
                    OnCorrectIncorrectConversation(character, message);

                    currentMessageCount++;

                    viewController.ShowView();
                    OnConversationStarted?.Invoke();
                    return;
                }
            }

            var randomBlurbs = conversationData.Messages.Where(m => m.MessageType == CharacterMessageType.RandomBlurb);
            if (randomBlurbs.TryGetRandom(out var blurb))
            {
                OnRandomBlurbChoiceConversation(character, blurb);

                currentMessageCount++;

                viewController.ShowView();
                OnConversationStarted?.Invoke();
                return;
            }

            StopConversation();
        }

        private void OnCorrectIncorrectConversation(CharacterActor character, CharacterMessageData message)
        {
            var conversationData = character.CharacterData.ConversationData;
            conversationData.RemoveMessage(message);

            var correctChoices = message.CorrectChoices
                .OrderBy(_ => Random.value)
                .Select(choice => new ConversationChoice(
                        isCorrect: true,
                        content: choice,
                        messageType: message.MessageType
                    )
                )
                .Take(maxCorrectMessages);

            var incorrectChoices = message.IncorrectChoices
                .OrderBy(_ => Random.value)
                .Select(choice => new ConversationChoice(
                        isCorrect: false,
                        content: choice,
                        messageType: message.MessageType
                    )
                )
                .Take(maxIncorrectMessages);

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
                        var conversationData = conversingWith.CharacterData.ConversationData;
                        var isMessagesLeft = conversationData.Messages.Any(m => m.MessageType == CharacterMessageType.CorrectIncorrect);
                        if (isMessagesLeft && currentMessageCount < currentMessageMax)
                        {
                            Converse(conversingWith);
                            return;
                        }

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
