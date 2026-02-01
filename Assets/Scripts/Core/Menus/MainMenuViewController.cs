using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Cursors;
using RIEVES.GGJ2026.Core.Pausing;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Core.Views;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RIEVES.GGJ2026.Core.Menus
{
    internal sealed class MainMenuViewController : ViewController<MainMenuView>
    {
        [Header("General")]
        [SerializeField]
        private InputActionReference showInputAction;

        [Header("Features")]
        [SerializeField]
        private bool isPauseOnShow;

        private ICursorSystem cursorSystem;
        private ISceneSystem sceneSystem;
        private IPauseSystem pauseSystem;

        protected override void Awake()
        {
            base.Awake();

            cursorSystem = GameManager.GetSystem<ICursorSystem>();
            sceneSystem = GameManager.GetSystem<ISceneSystem>();
            pauseSystem = GameManager.GetSystem<IPauseSystem>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (showInputAction)
            {
                showInputAction.action.performed += OnShowPerformed;
            }

            View.OnResumeClicked += OnResumeClicked;
            View.OnStartClicked += OnStartClicked;
            View.OnExitClicked += OnExitClicked;
            View.OnMainMenuClicked += OnMainMenuClicked;
            View.OnRestartClicked += OnRestartClicked;
            View.OnShowEntered += OnViewShown;
            View.OnHideEntered += OnViewHidden;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (showInputAction)
            {
                showInputAction.action.performed -= OnShowPerformed;
            }

            View.OnResumeClicked -= OnResumeClicked;
            View.OnStartClicked -= OnStartClicked;
            View.OnExitClicked -= OnExitClicked;
            View.OnMainMenuClicked -= OnMainMenuClicked;
            View.OnRestartClicked -= OnRestartClicked;
            View.OnShowEntered -= OnViewShown;
            View.OnHideEntered -= OnViewHidden;
        }

        private void OnShowPerformed(InputAction.CallbackContext context)
        {
            switch (View.State)
            {
                case ViewState.Hiding:
                case ViewState.Hidden:
                {
                    View.Show();
                    break;
                }
                case ViewState.Showing:
                case ViewState.Shown:
                {
                    View.Hide();
                    break;
                }
            }
        }

        private void OnResumeClicked()
        {
            View.Hide();
        }

        private void OnStartClicked()
        {
            sceneSystem.LoadGameplayScene();
        }

        private void OnExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnMainMenuClicked()
        {
            sceneSystem.LoadMainMenuScene();
        }

        private void OnRestartClicked()
        {
            sceneSystem.ReloadScene();
        }

        private void OnViewShown()
        {
            cursorSystem.UnLockCursor();

            if (isPauseOnShow)
            {
                pauseSystem.PauseGame();
            }
        }

        private void OnViewHidden()
        {
            cursorSystem.LockCursor();

            if (isPauseOnShow)
            {
                pauseSystem.ResumeGame();
            }
        }
    }
}
