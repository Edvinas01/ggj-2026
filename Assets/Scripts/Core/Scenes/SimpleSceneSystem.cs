using CHARK.GameManagement;
using CHARK.GameManagement.Systems;
using CHARK.ScriptableScenes;
using CHARK.ScriptableScenes.Events;
using RIEVES.GGJ2026.Core.Pausing;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Scenes
{
    public sealed class SimpleSceneSystem : MonoSystem, ISceneSystem
    {
        [Header("General")]
        [SerializeField]
        private ScriptableSceneController controller;

        [Header("Scenes")]
        [SerializeField]
        private ScriptableSceneCollection menuSceneCollection;

        [SerializeField]
        private ScriptableSceneCollection startingSceneCollection;

        [SerializeField]
        private ScriptableSceneCollection gameVictorySceneCollection;

        [SerializeField]
        private ScriptableSceneCollection gameOverSceneCollection;

        private IPauseSystem pauseSystem;

        public bool IsLoading => controller.IsLoading;

        public override void OnInitialized()
        {
            pauseSystem = GameManager.GetSystem<IPauseSystem>();

            controller.CollectionEvents.OnLoadEntered += OnLoadEntered;
            controller.CollectionEvents.OnLoadExited += OnLoadExited;

            controller.CollectionEvents.OnUnloadEntered += OnUnloadEntered;
        }

        public override void OnDisposed()
        {
            controller.CollectionEvents.OnLoadEntered -= OnLoadEntered;
            controller.CollectionEvents.OnLoadExited -= OnLoadExited;

            controller.CollectionEvents.OnUnloadEntered -= OnUnloadEntered;
        }

        public bool TryGetLoadedCollection(out ScriptableSceneCollection collection)
        {
            return controller.TryGetLoadedSceneCollection(out collection);
        }

        public bool IsStartingScene(ScriptableSceneCollection collection)
        {
            return startingSceneCollection == collection || menuSceneCollection == collection;
        }

        public void LoadInitialScene()
        {
            controller.LoadInitialSceneCollection();
        }

        public void ReloadScene()
        {
            controller.ReloadLoadedSceneCollection();
        }

        public void LoadMainMenuScene()
        {
            controller.LoadSceneCollection(menuSceneCollection);
        }

        public void LoadGameplayScene()
        {
            controller.LoadSceneCollection(startingSceneCollection);
        }

        public void LoadScene(ScriptableSceneCollection collection)
        {
            controller.LoadSceneCollection(collection);
        }

        public void LoadGameVictoryScene()
        {
            controller.LoadSceneCollection(gameVictorySceneCollection);
        }

        public void LoadGameOverScene()
        {
            controller.LoadSceneCollection(gameOverSceneCollection);
        }

        private static void OnLoadEntered(CollectionLoadEventArgs args)
        {
            var message = new SceneLoadEnteredMessage(args.Collection);
            GameManager.Publish(message);
        }

        private void OnLoadExited(CollectionLoadEventArgs args)
        {
            pauseSystem.ResumeGame();

            var message = new SceneLoadExitedMessage(args.Collection);
            GameManager.Publish(message);
        }

        private void OnUnloadEntered(CollectionUnloadEventArgs args)
        {
            var message = new SceneUnloadEnteredMessage(args.Collection);
            GameManager.Publish(message);
        }
    }
}
