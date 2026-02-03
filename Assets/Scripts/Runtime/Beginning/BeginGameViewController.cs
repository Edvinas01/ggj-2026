using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Audio;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Core.Views;

namespace RIEVES.GGJ2026.Runtime.Beginning
{
    internal sealed class BeginGameViewController : ViewController<BeginGameView>
    {
        private IAudioSystem audioSystem;
        private ISceneSystem sceneSystem;

        private bool isAudioLoaded;

        protected override void Awake()
        {
            base.Awake();

            sceneSystem = GameManager.GetSystem<ISceneSystem>();
            audioSystem = GameManager.GetSystem<IAudioSystem>();
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

        protected override void Update()
        {
            base.Update();

            if (isAudioLoaded)
            {
                return;
            }

            if (audioSystem.IsLoading)
            {
                return;
            }

            isAudioLoaded = true;

            View.IsLoadingTextEnabled = false;
            View.IsBeginButtonEnabled = true;
        }

        private void OnBeginClicked()
        {
            sceneSystem.LoadMainMenuScene();
        }
    }
}
