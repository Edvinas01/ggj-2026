using CHARK.GameManagement;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Gameplay
{
    internal sealed class GameStateTrigger : MonoBehaviour
    {
        private IGameStateSystem gameStateSystem;

        private void Awake()
        {
            gameStateSystem = GameManager.GetSystem<IGameStateSystem>();
        }

        public void StartGame()
        {
            gameStateSystem.StartGame();
        }

        public void StopGame()
        {
            gameStateSystem.StopGame();
        }
    }
}
