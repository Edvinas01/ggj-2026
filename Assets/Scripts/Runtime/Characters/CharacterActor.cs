using CHARK.GameManagement;
using RIEVES.GGJ2026.Runtime.Agents;
using UnityEngine;
using UnityEngine.AI;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterActor : MonoBehaviour, IAgent
    {
        [Header("General")]
        [SerializeField]
        private CharacterData data;

        [Header("AI")]
        [SerializeField]
        private NavMeshAgent agent;

        private AgentSystem agentSystem;

        private void Awake()
        {
            agentSystem = GameManager.GetSystem<AgentSystem>();
        }

        private void OnEnable()
        {
            agentSystem.AddAgent(this);
        }

        private void OnDisable()
        {
            agentSystem.RemoveAgent(this);
        }

        public void Initialize(CharacterData newData)
        {
            data = newData;
            // var block = new MaterialPropertyBlock();
            // block.SetTexture(texturePropertyId, data.Image);
            // bodyRenderer.SetPropertyBlock(block);
        }
    }
}
