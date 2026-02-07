using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Audio;
using RIEVES.GGJ2026.Core.Cursors;
using RIEVES.GGJ2026.Core.Gameplay;
using RIEVES.GGJ2026.Core.Input;
using RIEVES.GGJ2026.Core.Interaction;
using RIEVES.GGJ2026.Core.Pausing;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Core.Settings;
using RIEVES.GGJ2026.Core.Transforms;
using RIEVES.GGJ2026.Runtime.Heat;
using RIEVES.GGJ2026.Runtime.Stats;
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

        [SerializeField]
        private HeatSystem heatSystem;

        [SerializeField]
        private StatsSystem statsSystem;

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
            AddSystem(heatSystem);
            AddSystem(statsSystem);

            AddSystem(new SimpleTransformSystem());
            AddSystem(new SimpleInteractionSystem());
        }

        protected override void OnStarted()
        {
            base.OnStarted();

            // Seems that we have automatic bank loading enabled, so just yeeting this.
            // audioSystem.LoadBanks();

            // The initial scene uses BeginGameViewController to start the game so that
            // audio works correctly in Web builds. We keep the same flow in Desktop as well to
            // avoid differences in builds.
        }

    }
}
