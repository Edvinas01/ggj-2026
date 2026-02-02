using System;
using System.Linq;
using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Utilities;
using RIEVES.GGJ2026.Runtime.Heat;
using RIEVES.GGJ2026.Runtime.Resources;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationController : MonoBehaviour
    {
        public enum ConversationResult
        {
            Correct,
            Incorrect,
            Neutral,
        }

        [Header("General")]
        [SerializeField]
        private ConversationViewController viewController;

        [SerializeField]
        private ResourceController resourceController;

        [Header("Features")]
        [Min(1)]
        [SerializeField]
        private int maxMessagesPerConvo = 2;

        [Min(0)]
        [SerializeField]
        private int maxCorrectMessages = 1;

        [Min(0)]
        [SerializeField]
        private int maxIncorrectMessages = 1;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onCorrectChoiceSelected;

        [SerializeField]
        private UnityEvent onIncorrectChoiceSelected;

        [SerializeField]
        private UnityEvent onRandomBlurbChoiceSelected;

        [SerializeField]
        private UnityEvent onHuntChoiceSelected;

        private HeatSystem heatSystem;
        private AgentSystem agentSystem;

        private int currentMessageCount;
        private int currentMessageMax;

        public CharacterActor conversingWith { get; private set; }

        public event Action OnConversationStarted;

        public event Action OnConversationStopped;

        private void Awake()
        {
            heatSystem = GameManager.GetSystem<HeatSystem>();
            agentSystem = GameManager.GetSystem<AgentSystem>();
        }

        private void OnEnable()
        {
            viewController.OnChoiceSelected += OnChoiceSelected;
            viewController.OnTextWriterFinished += OnTextWriterFinished;
            viewController.OnTextWriterStarted += OnTextWriterStarted;
        }

        private void OnDisable()
        {
            viewController.OnChoiceSelected -= OnChoiceSelected;
            viewController.OnTextWriterFinished -= OnTextWriterFinished;
            viewController.OnTextWriterStarted -= OnTextWriterStarted;
        }

        public void StartConversation(CharacterActor character)
        {
            conversingWith = character;
            currentMessageCount = 0;
            currentMessageMax = Random.Range(1, (int)(maxMessagesPerConvo * heatSystem.CurrentHeat));

            Converse(character);
        }

        float conversationTimer = 0f;
        float convCooldowntimer = -100f;
        float defendedCooldowntimer = -100f;
        float defendChance = 0.9f;

        void Update()
        {
            if (conversingWith && Time.time > conversationTimer)
            {
                StopConversation(ConversationResult.Incorrect);
                return;
            }

            if (Time.time < convCooldowntimer)
                return;

            foreach (var agent in agentSystem.agents)
            {
                var dist = (agent.transform.position - transform.position).sqrMagnitude;
                if (dist < 1f)
                {
                    if (agent.WantsToTalk && conversingWith != agent)
                    {
                        // Handle interrupting conversations.
                        if (conversingWith != null)
                        {
                            // Don't interrupt when guarding.
                            if (agent.CurrentState == CharacterState.Guarding)
                                continue;

                            if (Time.time < defendedCooldowntimer)
                                return;

                            var rng = Random.value;
                            if (rng < defendChance)
                            {
                                defendedCooldowntimer = Time.time + 0.5f;
                                return;
                            }

                            StopConversation(ConversationResult.Neutral);
                        }

                        agent.StartConversation(transform);
                        StartConversation(agent);
                        break;
                    }
                }
            }
        }

        public void StopConversation(ConversationResult result, bool hunter = false)
        {
            convCooldowntimer = Time.time + 3f;
            viewController.HideView();

            if (conversingWith)
            {
                conversingWith.CharacterData.ConversationData.ConversedCount++;

                switch (result)
                {
                    case ConversationResult.Correct:
                        {
                            onCorrectChoiceSelected.Invoke();
                            resourceController.AddAlcohol(conversingWith.CharacterData.AddsAlcohol);
                            conversingWith.ConversationStoppedCorrect();
                            break;
                        }
                    case ConversationResult.Incorrect:
                        {
                            if (hunter)
                                onHuntChoiceSelected.Invoke();
                            else
                                onIncorrectChoiceSelected.Invoke();

                            resourceController.UseAlcohol(conversingWith.CharacterData.RemovesAlcohol);
                            conversingWith.ConversationStoppedIncorrect();
                            break;
                        }
                    case ConversationResult.Neutral:
                    default:
                        {
                            onRandomBlurbChoiceSelected.Invoke();
                            conversingWith.ConversationStoppedNeutral();
                            break;
                        }
                }
            }

            if (conversingWith)
            {
                conversingWith.StopVoice();
            }

            currentMessageCount = 0;
            conversingWith = null;

            OnConversationStopped?.Invoke();
        }

        public bool IsContainsMessages(CharacterActor character)
        {
            if (character.CurrentState == CharacterState.Hunting)
            {
                return true;
            }

            var conversationData = character.CharacterData.ConversationData;
            var messages = conversationData.Messages.ToList();

            if (conversationData.ConversedCount <= 0 && messages.Any(m => m.MessageType == CharacterMessageType.CorrectIncorrect))
            {
                return true;
            }

            return messages.Any(m => m.MessageType == CharacterMessageType.RandomBlurb);
        }

        private void Converse(CharacterActor character)
        {
            convCooldowntimer = Time.time + 1.5f;

            var conversationData = character.CharacterData.ConversationData;
            if (character.CurrentState == CharacterState.Hunting)
            {
                if (conversationData.Messages.TryGetRandom(out var message))
                {
                    OnHuntConversation(character, message);

                    currentMessageCount++;

                    viewController.ShowView();
                    OnConversationStarted?.Invoke();

                    conversationTimer = Time.time + character.CharacterData.AgitatedConversationDuration;
                    return;
                }
            }

            conversationTimer = Time.time + character.CharacterData.ConversationDuration;

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

            StopConversation(ConversationResult.Neutral);
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
                .Take((int)(maxIncorrectMessages * heatSystem.CurrentHeat));

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

        private void OnHuntConversation(CharacterActor character, CharacterMessageData message)
        {
            viewController.Initialize(
                title: character.CharacterData.CharacterName,
                content: message.HuntMessage,
                choices: new[]
                {
                    new ConversationChoice(
                        messageType: CharacterMessageType.Hunter,
                        isCorrect: false,
                        content: null
                    ),
                }
            );
        }

        private void OnTextWriterStarted()
        {
            if (conversingWith)
            {
                conversingWith.PlayVoice();
            }
        }

        private void OnTextWriterFinished()
        {
            if (conversingWith)
            {
                conversingWith.StopVoice();
            }
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

                            if (conversingWith.CharacterData.AddsAlcohol <= 0)
                            {
                                StopConversation(ConversationResult.Neutral);
                                return;
                            }
                        }

                        StopConversation(choice.IsCorrect ? ConversationResult.Correct : ConversationResult.Incorrect);
                        return;
                    }
                case CharacterMessageType.RandomBlurb:
                    {
                        StopConversation(ConversationResult.Neutral);
                        return;
                    }
                case CharacterMessageType.Hunter:
                    {
                        StopConversation(ConversationResult.Incorrect, true);
                        return;
                    }
            }

            StopConversation(ConversationResult.Neutral);
        }
    }
}
