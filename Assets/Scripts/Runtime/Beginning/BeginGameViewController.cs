using System;
using System.Collections;
using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Audio;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Core.Views;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Beginning
{
    internal sealed class BeginGameViewController : ViewController<BeginGameView>
    {
        [SerializeField]
        private float maxLoadDuration = 10f;

        [SerializeField]
        private float loadTickDelay = 0.1f;

        private IAudioSystem audioSystem;
        private ISceneSystem sceneSystem;

        protected override void Awake()
        {
            base.Awake();

            sceneSystem = GameManager.GetSystem<ISceneSystem>();
            audioSystem = GameManager.GetSystem<IAudioSystem>();
        }

        protected override void Start()
        {
            base.Start();

            View.IsBeginButtonEnabled = false;
            View.IsLoadingTextEnabled = true;

            StartCoroutine(
                WaitForGameLoadRoutine(() =>
                    {
                        View.IsBeginButtonEnabled = true;
                        View.IsLoadingTextEnabled = false;
                    }
                )
            );
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            View.OnBeginClicked += OnBeginClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            View.OnBeginClicked -= OnBeginClicked;
        }

        private void OnBeginClicked()
        {
            sceneSystem.LoadMainMenuScene();
        }

        private IEnumerator WaitForGameLoadRoutine(Action onLoaded)
        {
            // We wait one frame before checking to reduce the chance of FMOD doing something funky.
            // Also, they suggest doing this in their examples...
            yield return null;

            var loadDuration = 0f;
            while (audioSystem.IsLoading && loadDuration < maxLoadDuration)
            {
                // We wait a bit longer here as there is no need to poll often.
                yield return new WaitForSeconds(loadTickDelay);
                loadDuration += loadTickDelay;
            }

            onLoaded?.Invoke();
        }
    }
}
