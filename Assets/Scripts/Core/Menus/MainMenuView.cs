using System;
using RIEVES.GGJ2026.Core.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RIEVES.GGJ2026.Core.Menus
{
    internal sealed class MainMenuView : View
    {
        [Header("Buttons")]
        [SerializeField]
        private MenuButtonElement resumeButton;

        [SerializeField]
        private MenuButtonElement startButton;

        [SerializeField]
        private MenuButtonElement restartButton;

        [SerializeField]
        private MenuButtonElement mainMenuButton;

        [SerializeField]
        private MenuButtonElement exitGameButton;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onShown;

        [SerializeField]
        private UnityEvent onHidden;

        private GameObject prevSelectedGameObject;

        public event Action OnResumeClicked;
        public event Action OnStartClicked;
        public event Action OnRestartClicked;
        public event Action OnMainMenuClicked;
        public event Action OnExitClicked;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (resumeButton)
            {
                resumeButton.OnClicked += OnResumeButtonClicked;
            }

            if (startButton)
            {
                startButton.OnClicked += OnStartButtonClicked;
            }

            if (restartButton)
            {
                restartButton.OnClicked += OnRestartButtonClicked;
            }

            if (mainMenuButton)
            {
                mainMenuButton.OnClicked += OnMainMenuButtonClicked;
            }

            if (exitGameButton)
            {
                exitGameButton.OnClicked += OnExitGameButtonClicked;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (resumeButton)
            {
                resumeButton.OnClicked -= OnResumeButtonClicked;
            }

            if (startButton)
            {
                startButton.OnClicked -= OnStartButtonClicked;
            }

            if (restartButton)
            {
                restartButton.OnClicked -= OnRestartButtonClicked;
            }


            if (mainMenuButton)
            {
                mainMenuButton.OnClicked -= OnMainMenuButtonClicked;
            }

            if (exitGameButton)
            {
                exitGameButton.OnClicked -= OnExitGameButtonClicked;
            }
        }

        protected override void Start()
        {
            base.Start();

#if UNITY_WEBGL && UNITY_EDITOR == false
            if (exitGameButton)
            {
                exitGameButton.gameObject.SetActive(false);
            }
#endif
        }

        private void OnResumeButtonClicked()
        {
            OnResumeClicked?.Invoke();
        }

        private void OnStartButtonClicked()
        {
            OnStartClicked?.Invoke();
        }

        private void OnExitGameButtonClicked()
        {
            OnExitClicked?.Invoke();
        }

        private void OnMainMenuButtonClicked()
        {
            OnMainMenuClicked?.Invoke();
        }

        private void OnRestartButtonClicked()
        {
            OnRestartClicked?.Invoke();
        }

        public override void SelectGameObject()
        {
            if (resumeButton)
            {
                EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
                return;
            }

            if (startButton)
            {
                EventSystem.current.SetSelectedGameObject(startButton.gameObject);
                return;
            }

            if (restartButton)
            {
                EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
                return;
            }

            if (mainMenuButton)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuButton.gameObject);
                return;
            }

            if (exitGameButton)
            {
                EventSystem.current.SetSelectedGameObject(exitGameButton.gameObject);
            }
        }
    }
}
