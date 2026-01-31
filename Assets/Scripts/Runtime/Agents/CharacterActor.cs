using System;
using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Interaction.Interactables;
using RIEVES.GGJ2026.Runtime.Movement;
using RIEVES.GGJ2026.Runtime.Characters;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using System.Data.Common;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace RIEVES.GGJ2026
{
    internal sealed class CharacterActor : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private CharacterData data;

        [SerializeField]
        private Interactable interactable;

        [SerializeField]
        private List<StateChangeTriggers> onStateChange = new();
        public CharacterState CurrentState { get; private set; } = CharacterState.Idling;
        public CharacterActivity CurrentActivity { get; private set; } = CharacterActivity.Idling;
        public PointOfInterest CurrentTarget { get; private set; }

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

        [Header("Events")]
        [SerializeField]
        private UnityEvent onConversationStarted;

        [SerializeField]
        private UnityEvent onConversationStopped;

        private CharacterData runtimeData;
        private AgentSystem agentSystem;

        public CharacterData CharacterData => runtimeData;

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

        float stateChangeTimer = -100f;
        float minPatienceDuration = 10f;
        float maxPatienceDuration = 20f;
        bool changingFromState = false;
        bool changingToState = false;

        public bool SetState(CharacterState newState)
        {
            if (!changingFromState)
            {
                var OldTrigger = onStateChange.FirstOrDefault(s => s.State == CurrentState);
                if (OldTrigger != null)
                {
                    OldTrigger.OnStateEnding.Invoke();
                    if (OldTrigger.EndDelay > 0)
                    {
                        stateChangeTimer = Time.time + OldTrigger.EndDelay;
                        CurrentState = newState;
                        changingFromState = true;
                        CurrentActivity = CharacterActivity.Idling;
                        CurrentTarget = null;
                        return false;
                    }
                }
            }

            if (!changingToState)
            {
                var NewTrigger = onStateChange.FirstOrDefault(s => s.State == newState);
                if (NewTrigger != null)
                {
                    NewTrigger.OnStateStarting.Invoke();
                    if (NewTrigger.StartDelay > 0)
                    {
                        stateChangeTimer = Time.time + NewTrigger.StartDelay;
                        changingToState = true;
                        CurrentActivity = CharacterActivity.Idling;
                        CurrentTarget = null;
                        return false;
                    }
                }
            }

            CurrentState = newState;
            changingFromState = false;
            changingToState = false;
            stateChangeTimer = Time.time + UnityEngine.Random.Range(minPatienceDuration, maxPatienceDuration);
            CurrentActivity = CharacterActivity.Idling;
            CurrentTarget = null;
            return true;
        }

        public void SetActivity(CharacterActivity newActivity)
        {
            CurrentActivity = newActivity;
        }

        private void Update()
        {
            if (changingFromState || changingToState)
            {
                if (stateChangeTimer < Time.time)
                {
                    SetState(CurrentState);
                }

                return;
            }

            if (CurrentState != CharacterState.Talking && stateChangeTimer < Time.time)
            {
                var currentTargetState = CurrentState;
                CurrentState = agentSystem.GetRandomState(this);
                stateChangeTimer = Time.time;
                Debug.Log($"Changing state due to patience timeout: {currentTargetState} -> {CurrentState}");
                if (currentTargetState != CurrentState)
                    SetState(CurrentState);
            }

            switch (CurrentState)
            {
                case CharacterState.Idling:
                case CharacterState.Talking:
                    CurrentActivity = CharacterActivity.Idling;
                    break;

                case CharacterState.Guarding:
                case CharacterState.Dancing:
                case CharacterState.Watching:
                    {
                        if (CurrentTarget == null)
                        {
                            var targetType = CurrentState == CharacterState.Guarding ? InterestType.Guard :
                                CurrentState == CharacterState.Watching ?
                                    InterestType.Watch :
                                    InterestType.Dancing;

                            var newTarget = agentSystem.PickRandomWaypoint(targetType);
                            if (newTarget != null)
                                MoveToPoint(newTarget);
                            else
                                SetState(agentSystem.GetRandomState(this));
                        }
                        else if (CurrentActivity != CharacterActivity.Walking)
                        {
                            var distanceSqr = (transform.position - CurrentTarget.transform.position).sqrMagnitude;
                            if (distanceSqr >= CurrentTarget.StayWithinRange * CurrentTarget.StayWithinRange)
                            {
                                if (CurrentTarget != null)
                                    MoveToPoint(CurrentTarget);
                                else
                                    SetState(agentSystem.GetRandomState(this));
                            }
                        }
                    }
                    break;
                case CharacterState.Hunting:
                    {
                        if (CurrentTarget == null || CurrentActivity != CharacterActivity.Walking)
                        {
                            var newTarget = agentSystem.PickRandomWaypoint(InterestType.Patrol);
                            if (newTarget != null)
                                MoveToPoint(newTarget);
                            else
                                SetState(agentSystem.GetRandomState(this));
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (CurrentActivity)
            {
                case CharacterActivity.Hunting:
                case CharacterActivity.Walking:
                    {
                        navMeshAgent.enabled = true;

                        var characterPosition = rigidBody.position;
                        movementPositionInput.TargetPosition = characterPosition;
                        var distanceSqr = (transform.position - CurrentTarget.transform.position).sqrMagnitude;
                        if (distanceSqr <= CurrentTarget.MoveWithinRange * CurrentTarget.MoveWithinRange)
                        {
                            switch (CurrentState)
                            {
                                case CharacterState.Watching:
                                    CurrentActivity = CharacterActivity.Watching;
                                    break;
                                case CharacterState.Guarding:
                                    CurrentActivity = CharacterActivity.Guarding;
                                    break;
                                case CharacterState.Dancing:
                                    CurrentActivity = CharacterActivity.Dancing;
                                    break;
                            }
                        }

                        var characterMoveDir = navMeshAgent.steeringTarget - characterPosition;
                        movementPositionInput.TargetPosition = characterPosition + characterMoveDir;

                        break;
                    }
                case CharacterActivity.Idling:
                    navMeshAgent.enabled = false;
                    break;
                default:
                    {
                        if (CurrentTarget != null && CurrentTarget.Facing)
                        {
                            var directionToTarget = (CurrentTarget.transform.position - transform.position).normalized;
                            var lookAtPosition = transform.position + new Vector3(directionToTarget.x, 0, directionToTarget.z);
                            movementPositionInput.TargetPosition = lookAtPosition;
                        }

                        break;
                    }
            }
        }

        void MoveToPoint(PointOfInterest target)
        {
            CurrentTarget = null;
            navMeshAgent.enabled = true;

            if (NavMesh.SamplePosition(target.transform.position, out var hit, 10f, NavMesh.AllAreas))
            {
                CurrentTarget = target;
                navMeshAgent.SetDestination(hit.position);
                CurrentActivity = CharacterActivity.Walking;
            }
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
            runtimeData.ConversationData.RemoveMessage(message);
        }

        public void ConversationStopped()
        {
            CurrentActivity = CharacterActivity.Idling;
            onConversationStopped.Invoke();
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
                SetState(CharacterState.Talking);
                controller.StartConversation(this);
                onConversationStarted.Invoke();
            }
        }

        private void OnInteractableSelectExited(InteractableSelectExitedArgs args)
        {
        }
    }
}
