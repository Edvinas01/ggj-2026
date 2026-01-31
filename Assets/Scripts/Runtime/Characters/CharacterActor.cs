using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Interaction.Interactables;
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

        [SerializeField]
        private Interactable interactable;

        [Header("Physics")]
        [SerializeField]
        private Rigidbody rigidBody;

        [Header("AI")]
        [SerializeField]
        private NavMeshAgent agent;

        private AgentSystem agentSystem;

        public string Name => data.CharacterName;

        private void Awake()
        {
            agentSystem = GameManager.GetSystem<AgentSystem>();

            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        private void OnEnable()
        {
            agentSystem.AddAgent(this);

            interactable.OnHoverEntered += OnInteractableHoverEntered;
            interactable.OnHoverExited += OnInteractableHoverExited;
            interactable.OnSelectEntered += OnInteractableSelectEntered;
            interactable.OnSelectExited += OnInteractableSelectExited;
        }

        private void OnDisable()
        {
            agentSystem.RemoveAgent(this);

            interactable.OnHoverEntered -= OnInteractableHoverEntered;
            interactable.OnHoverExited -= OnInteractableHoverExited;
            interactable.OnSelectEntered -= OnInteractableSelectEntered;
            interactable.OnSelectExited -= OnInteractableSelectExited;
        }

        private void LateUpdate()
        {
            agent.nextPosition = rigidBody.position;
        }
        public void Initialize(CharacterData newData)
        {
            data = newData;
            // var block = new MaterialPropertyBlock();
            // block.SetTexture(texturePropertyId, data.Image);
            // bodyRenderer.SetPropertyBlock(block);
        }

        private void OnInteractableHoverEntered(InteractableHoverEnteredArgs args)
        {
        }

        private void OnInteractableHoverExited(InteractableHoverExitedArgs args)
        {
        }

        private void OnInteractableSelectEntered(InteractableSelectEnteredArgs args)
        {
            if (args.Interactor is not Component component)
            {
                return;
            }

            var controller = component.GetComponentInParent<ConversationController>();
            if (controller)
            {
                controller.StartConversation(this);
            }

            args.Interactor.Deselect();
        }

        private void OnInteractableSelectExited(InteractableSelectExitedArgs args)
        {
        }
    }
}
