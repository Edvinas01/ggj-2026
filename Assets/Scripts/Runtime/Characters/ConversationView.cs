using System;
using RIEVES.GGJ2026.Core.Menus;
using RIEVES.GGJ2026.Core.Views;
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
        private string defaultButtonText = "Gerai";

        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private TMP_Text contentText;

        public string TitleText
        {
            set => titleText.text = value;
        }

        public string ContentText
        {
            set => contentText.text = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public void AddChoice(ConversationChoice choice, Action onClicked)
        {
            var element = Instantiate(choiceButtonPrefab, choiceParent);
            element.OnClicked += onClicked;

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
            for (var index = 0; index < choiceParent.childCount; index++)
            {
                var childTransform = choiceParent.GetChild(index);
                Destroy(childTransform.gameObject);
            }
        }
    }
}
