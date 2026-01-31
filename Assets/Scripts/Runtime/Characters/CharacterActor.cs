using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Interaction.Interactables;
using RIEVES.GGJ2026.Runtime.Agents;
using RIEVES.GGJ2026.Runtime.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterActor : MonoBehaviour, IAgent
    {
        [Header("General")]
        [SerializeField]
        private CharacterData data;

        [SerializeField]
        private Interactable interactable;

        [Header("Rendering")]
        [SerializeField]
        private Renderer frontRenderer;

        [SerializeField]
        private Renderer backRenderer;

        [SerializeField]
        private string texturePropertyId = "_BaseMap";

        [Header("Movement")]
        [FormerlySerializedAs("movementPointerInput")]
        [SerializeField]
        private MovementPositionInputProvider movementPositionInput;

        [Header("Physics")]
        [SerializeField]
        private Rigidbody rigidBody;

        [Header("AI")]
        [FormerlySerializedAs("agent")]
        [SerializeField]
        private NavMeshAgent navMeshAgent;

        private CharacterData runtimeData;
        private AgentSystem agentSystem;

        public CharacterData CharacterData => runtimeData;

        public Vector3 TargetPosition
        {
            set
            {
                if (navMeshAgent.enabled == false)
                {
                    return;
                }

                if (NavMesh.SamplePosition(value, out var hit, 10f, NavMesh.AllAreas))
                {
                    navMeshAgent.SetDestination(hit.position);
                }
            }
        }

        private void Awake()
        {
            agentSystem = GameManager.GetSystem<AgentSystem>();

            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
        }

        private void Start()
        {
            if (data)
            {
                Initialize(data);
            }
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

        private void Update()
        {
            var characterPosition = rigidBody.position;
            var characterMoveDir = navMeshAgent.steeringTarget - characterPosition;

            movementPositionInput.TargetPosition = characterPosition + characterMoveDir;
        }

        private void LateUpdate()
        {
            navMeshAgent.nextPosition = rigidBody.position;
        }

        public void Initialize(CharacterData newData)
        {
            runtimeData = Instantiate(newData);

            var block = new MaterialPropertyBlock();

            block.SetTexture(texturePropertyId, data.FrontTexture);
            frontRenderer.SetPropertyBlock(block);

            block.SetTexture(texturePropertyId, data.BackTexture);
            backRenderer.SetPropertyBlock(block);
        }

        public void RemoveMessage(CharacterMessageData message)
        {
            runtimeData.Conversation.RemoveMessage(message);
        }

        public void ConversationStopped()
        {
            navMeshAgent.enabled = true;
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
                navMeshAgent.enabled = false;
                controller.StartConversation(this);
            }
        }

        private void OnInteractableSelectExited(InteractableSelectExitedArgs args)
        {
        }
    }
}
