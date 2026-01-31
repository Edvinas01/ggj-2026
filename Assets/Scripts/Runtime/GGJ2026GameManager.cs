using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Audio;
using RIEVES.GGJ2026.Core.Cursors;
using RIEVES.GGJ2026.Core.Gameplay;
using RIEVES.GGJ2026.Core.Input;
using RIEVES.GGJ2026.Core.Pausing;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Core.Settings;
using RIEVES.GGJ2026.Core.Transforms;
using RIEVES.GGJ2026.Runtime.Agents;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime
{
    internal sealed class GGJ2026GameManager : GameManager
    {
        [Header("Systems")]
        [SerializeField]
        private FMODAudioSystem audioSystem;

        [SerializeField]
        private SimpleCursorSystem cursorSystem;

        [SerializeField]
        private SimpleInputSystem inputSystem;

        [SerializeField]
        private SimplePauseSystem pauseSystem;

        [SerializeField]
        private JsonSettingsSystem settingsSystem;

        [SerializeField]
        private SimpleSceneSystem sceneSystem;

        [SerializeField]
        private SimpleGameStateSystem gameStateSystem;

        [SerializeField]
        private AgentSystem agentSystem;

        protected override void OnBeforeInitializeSystems()
        {
            AddSystem(audioSystem);
            AddSystem(cursorSystem);
            AddSystem(inputSystem);
            AddSystem(pauseSystem);
            AddSystem(settingsSystem);
            AddSystem(sceneSystem);
            AddSystem(sceneSystem);
            AddSystem(gameStateSystem);
            AddSystem(agentSystem);

            AddSystem(new SimpleTransformSystem());
        }

        protected override void OnStarted()
        {
            base.OnStarted();

            audioSystem.LoadBanks();

#if UNITY_WEBGL
            StartCoroutine(LoadGameRoutine());
#else
            sceneSystem.LoadInitialScene();
#endif
        }

#if UNITY_WEBGL
        private System.Collections.IEnumerator LoadGameRoutine()
        {
            if (audioSystem.IsLoading)
            {
                yield return null;
            }

            // TODO: scuffed workaround for WebGL not playing audio in main menu, oh well...
            yield return new WaitForSeconds(1f);
            sceneSystem.LoadInitialScene();
        }
#endif
    }
}
