using System;
using System.Collections.Generic;
using System.Linq;
using RIEVES.GGJ2026.Core.Menus;
using RIEVES.GGJ2026.Core.Views;
using TMPEffects.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationView : View
    {
        [Header("Elements")]
        [SerializeField]
        private MenuButtonElement choiceButtonPrefab;

        [SerializeField]
        private RectTransform choiceParent;

        [Header("Text")]
        [SerializeField]
        private TMPWriter tmpWriter;

        [SerializeField]
        private string defaultButtonText = "Gerai";

        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private TMP_Text contentText;

        [SerializeField]
        private TMP_Text timeText;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onTextWriterStarted;

        [SerializeField]
        private UnityEvent onTextWriterFinished;

        [SerializeField]
        private UnityEvent onTimerChanged;

        private readonly List<MenuButtonElement> elements = new();

        public event Action OnTextWriterStarted;

        public event Action OnTextWriterFinished;


        public string TitleText
        {
            set => titleText.text = value;
        }

        public string ContentText
        {
            set => contentText.text = value;
        }

        public int RemainingTime
        {
            set
            {
                timeText.text = $"{value.ToString()}s";
                onTimerChanged.Invoke();
            }
        }

        public bool IsRemainingTimeEnabled
        {
            set => timeText.gameObject.SetActive(value);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            tmpWriter.OnStartWriter.AddListener(OnStartWriter);
            tmpWriter.OnFinishWriter.AddListener(OnFinishWriter);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            tmpWriter.OnStartWriter.RemoveListener(OnStartWriter);
            tmpWriter.OnFinishWriter.RemoveListener(OnFinishWriter);
        }

        public void AddChoice(ConversationChoice choice, Action onClicked)
        {
            var element = Instantiate(choiceButtonPrefab, choiceParent);
            element.OnClicked += onClicked;
            elements.Add(element);

            if (string.IsNullOrWhiteSpace(choice.Content))
            {
                element.Text = defaultButtonText;
            }
            else
            {
                element.Text = choice.Content;
            }
        }

        public void ClearChoices()
        {
            foreach (var element in elements)
            {
                Destroy(element.gameObject);
            }

            elements.Clear();
        }

        public override void SelectGameObject()
        {
            var element = elements.FirstOrDefault();
            if (element)
            {
                GameObjectToSelect = element.gameObject;
            }

            base.SelectGameObject();
        }

        private void OnStartWriter(TMPWriter writer)
        {
            if (State is not (ViewState.Showing or ViewState.Shown))
            {
                return;
            }

            OnTextWriterStarted?.Invoke();
            onTextWriterStarted.Invoke();
        }

        private void OnFinishWriter(TMPWriter writer)
        {
            OnTextWriterFinished?.Invoke();
            onTextWriterFinished.Invoke();
        }

        protected override void OnViewHideEntered()
        {
            base.OnViewHideEntered();

            if (tmpWriter.IsWriting)
            {
                tmpWriter.StopWriter();
            }
        }
    }
}
