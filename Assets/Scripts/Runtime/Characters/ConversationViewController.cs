using System;
using System.Collections.Generic;
using RIEVES.GGJ2026.Core.Views;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationViewController : ViewController<ConversationView>
    {
        private Func<float> remainingTimeProvider;
        private int remainingSecondsPrev;
        private bool isInitialized;

        public event Action<ConversationChoice> OnChoiceSelected;

        public event Action OnTextWriterStarted;

        public event Action OnTextWriterFinished;

        protected override void OnEnable()
        {
            base.OnEnable();

            View.OnTextWriterFinished += OnViewTextWriterFinished;
            View.OnTextWriterStarted += OnViewTextWriterStarted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            View.OnTextWriterFinished -= OnViewTextWriterFinished;
            View.OnTextWriterStarted -= OnViewTextWriterStarted;
        }

        protected override void Update()
        {
            base.Update();

            if (isInitialized && View.State == ViewState.Shown)
            {
                var remainingTime = remainingTimeProvider?.Invoke() ?? 0f;
                var remainingSecondsNext = Mathf.CeilToInt(remainingTime);

                if (remainingSecondsPrev != remainingSecondsNext)
                {
                    View.RemainingTime = remainingSecondsNext;
                    View.IsRemainingTimeEnabled = remainingSecondsNext > 0;

                    remainingSecondsPrev = remainingSecondsNext;
                }
            }
        }

        public void Initialize(
            string title,
            string content,
            Func<float> newRemainingTimeProvider,
            IReadOnlyList<ConversationChoice> choices
        )
        {
            isInitialized = true;
            remainingTimeProvider = newRemainingTimeProvider;
            remainingSecondsPrev = Mathf.CeilToInt(remainingTimeProvider?.Invoke() ?? 0);

            View.TitleText = title;
            View.ContentText = content;
            View.RemainingTime = remainingSecondsPrev;
            View.IsRemainingTimeEnabled = remainingSecondsPrev > 0;

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

        private void OnViewTextWriterFinished()
        {
            OnTextWriterFinished?.Invoke();
        }

        private void OnViewTextWriterStarted()
        {
            OnTextWriterStarted?.Invoke();
        }
    }
}
