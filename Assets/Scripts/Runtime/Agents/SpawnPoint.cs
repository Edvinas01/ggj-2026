using UnityEngine;
using CHARK.GameManagement;

namespace RIEVES.GGJ2026
{
    public class SpawnPoint : MonoBehaviour
    {
        private AgentSystem agentSystem;

        private void Awake()
        {
            agentSystem = GameManager.GetSystem<AgentSystem>();
        }

        private void OnEnable()
        {
            agentSystem.AddSpawnPoint(this);
        }

        private void OnDisable()
        {
            agentSystem.RemoveSpawnPoint(this);
        }
    }
}
