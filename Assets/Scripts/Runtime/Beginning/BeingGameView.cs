using System;
using RIEVES.GGJ2026.Core.Menus;
using RIEVES.GGJ2026.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RIEVES.GGJ2026.Runtime.Beginning
{
    internal sealed class BeginGameView : View
    {
        [SerializeField]
        private TMP_Text loadingText;

        [SerializeField]
        private MenuButtonElement beginButton;

        public bool IsBeginButtonEnabled
        {
            set
            {
                beginButton.SetInteractable(value);

                if (value)
                {
                    EventSystem.current.SetSelectedGameObject(beginButton.gameObject);
                }
            }
        }

        public bool IsLoadingTextEnabled
        {
            set => loadingText.enabled = value;
        }

        public event Action OnBeginClicked;

        protected override void OnEnable()
        {
            base.OnEnable();

            beginButton.OnClicked += OnBeginButtonClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            beginButton.OnClicked -= OnBeginButtonClicked;
        }

        private void OnBeginButtonClicked()
        {
            OnBeginClicked?.Invoke();
        }
    }
}
