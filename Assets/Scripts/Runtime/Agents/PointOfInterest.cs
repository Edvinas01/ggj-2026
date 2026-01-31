using UnityEngine;
using CHARK.GameManagement;

namespace RIEVES.GGJ2026
{
    public class PointOfInterest : MonoBehaviour
    {
        public InterestType InterestType = InterestType.Idle;
        public bool Facing = false;
        public float StayWithinRange = 3f;
        public float MoveWithinRange = 0.1f;

        private AgentSystem agentSystem;

        private void Awake()
        {
            agentSystem = GameManager.GetSystem<AgentSystem>();
        }

        private void OnEnable()
        {
            agentSystem.AddPointOfInterest(this);
        }

        private void OnDisable()
        {
            agentSystem.RemovePointOfInterest(this);
        }
    }
}
