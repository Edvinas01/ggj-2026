using CHARK.GameManagement.Systems;

namespace RIEVES.GGJ2026.Core.Gameplay
{
    internal interface IGameStateSystem : ISystem, IUpdateListener
    {
        public void StartGame();

        public void StopGame();
    }
}
