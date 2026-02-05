using System;
using RIEVES.GGJ2026.Core.Menus;
using RIEVES.GGJ2026.Core.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RIEVES.GGJ2026.Runtime.Intro
{
    internal sealed class IntroView : View
    {
        [SerializeField]
        private MenuButtonElement startGameButton;

        public event Action OnStartGame;

        protected override void OnEnable()
        {
            base.OnEnable();

            startGameButton.OnClicked += OnStartGameClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            startGameButton.OnClicked -= OnStartGameClicked;
        }

        private void OnStartGameClicked()
        {
            OnStartGame?.Invoke();
        }

        public void ShowStartGameButton()
        {
            startGameButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(startGameButton.gameObject);
        }
    }
}
