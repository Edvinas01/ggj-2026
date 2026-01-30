using System.Collections.Generic;
using System.Linq;
using CHARK.GameManagement;
using CHARK.GameManagement.Systems;
using RIEVES.GGJ2026.Core.Scenes;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Gameplay
{
    public sealed class SimpleGameStateSystem : MonoSystem, IGameStateSystem
    {
        [SerializeField]
        private List<GameState> states = new();

        private GameState startingState;
        private GameState currentState;

        private GameState State
        {
            get => currentState;
            set
            {
                var oldState = currentState;
                var newState = value;

                if (oldState != null && oldState == newState)
                {
                    return;
                }

                oldState?.Exit();
                currentState = newState;
                newState?.Enter();

                OnStateChanged(oldState, newState);
            }
        }

        public override void OnInitialized()
        {
            GameManager.AddListener<SceneLoadEnteredMessage>(OnSceneLoadEntered);
            GameManager.AddListener<SceneUnloadEnteredMessage>(OnSceneUnloadEntered);
        }

        public override void OnDisposed()
        {
            GameManager.RemoveListener<SceneLoadEnteredMessage>(OnSceneLoadEntered);
            GameManager.RemoveListener<SceneUnloadEnteredMessage>(OnSceneUnloadEntered);
        }

        public void OnUpdated(float deltaTime)
        {
            if (State == false)
            {
                return;
            }

            State = State.Update();
        }

        public void StartGame()
        {
            foreach (var state in states)
            {
                state.Initialize();
            }

            State = states.First();
        }

        public void StopGame()
        {
            foreach (var state in states)
            {
                state.Dispose();
            }

            State = null;
        }

        private static void OnStateChanged(GameState statePrev, GameState stateNext)
        {
            Debug.Log($"New state {stateNext?.Name}");
        }


        private void OnSceneLoadEntered(SceneLoadEnteredMessage message)
        {
            // InitializeStateMachine();
        }

        private void OnSceneUnloadEntered(SceneUnloadEnteredMessage message)
        {
            StopGame();
        }
    }
}
