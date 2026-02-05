using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Core.Views;

namespace RIEVES.GGJ2026.Runtime.Intro
{
    internal sealed class IntroViewController : ViewController<IntroView>
    {
        private ISceneSystem sceneSystem;

        protected override void Awake()
        {
            base.Awake();

            sceneSystem = GameManager.GetSystem<ISceneSystem>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            View.OnStartGame += OnStartGame;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            View.OnStartGame -= OnStartGame;
        }

        private void OnStartGame()
        {
            sceneSystem.LoadGameplayScene();
        }
    }
}
