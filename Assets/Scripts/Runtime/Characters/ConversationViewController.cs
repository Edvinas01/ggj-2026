using System;
using System.Collections.Generic;
using FMODUnity;
using RIEVES.GGJ2026.Core.Views;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationViewController : ViewController<ConversationView>
    {
        [SerializeField]
        private StudioEventEmitter voiceAudioEmitter;

        public event Action<ConversationChoice> OnChoiceSelected;

        protected override void OnEnable()
        {
            base.OnEnable();

            View.OnTextWriterFinished += OnTextWriterFinished;
            View.OnTextWriterStarted += OnTextWriterStarted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            View.OnTextWriterFinished -= OnTextWriterFinished;
            View.OnTextWriterStarted -= OnTextWriterStarted;
        }

        public void Initialize(
            string title,
            string content,
            EventReference eventReference,
            IReadOnlyList<ConversationChoice> choices
        )
        {
            View.TitleText = title;
            View.ContentText = content;
            voiceAudioEmitter.EventReference = eventReference;

            View.ClearChoices();

            foreach (var choice in choices)
            {
                View.AddChoice(choice, () => OnChoiceSelected?.Invoke(choice));
            }
        }

        public override void ShowView()
        {
            if (View.State is ViewState.Showing or ViewState.Shown)
            {
                View.SelectGameObject();
            }

            base.ShowView();
        }

        private void OnTextWriterFinished()
        {
            if (voiceAudioEmitter.EventReference.IsNull)
            {
                return;
            }

            voiceAudioEmitter.Stop();
        }

        private void OnTextWriterStarted()
        {
            if (voiceAudioEmitter.EventReference.IsNull)
            {
                return;
            }

            voiceAudioEmitter.Play();
        }
    }
}
