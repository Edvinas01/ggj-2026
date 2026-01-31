using System;
using System.Collections.Generic;
using System.Linq;
using RIEVES.GGJ2026.Core.Menus;
using RIEVES.GGJ2026.Core.Views;
using TMPro;
using UnityEngine;

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

        private readonly List<MenuButtonElement> elements = new();

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
    }
}
