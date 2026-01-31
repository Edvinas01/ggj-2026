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
        public CharacterAnimationState CurrentAnimationState { get; private set; } = CharacterAnimationState.Idling;
        private CharacterAnimationState TransitioningAnimationState = CharacterAnimationState.Idling;
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
        Transform interactingWith;

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
        bool isInteractingWithPlayer = false;

        public bool SetAnimationState(CharacterAnimationState newState)
        {
            if (newState == CurrentAnimationState)
                return true;

            if (changingToState && newState != TransitioningAnimationState)
            {
                var oldTrigger = onStateChange.FirstOrDefault(s => s.State == TransitioningAnimationState);
                if (oldTrigger != null)
                {
                    oldTrigger.OnStateEnding?.Invoke();
                    if (oldTrigger.EndDelay > 0)
                    {
                        stateChangeTimer = Time.time + oldTrigger.EndDelay;
                        TransitioningAnimationState = newState;
                        changingFromState = true;
                        changingToState = false;
                        return false;
                    }
                }
            }

            if (!changingFromState)
            {
                var oldTrigger = onStateChange.FirstOrDefault(s => s.State == CurrentAnimationState);
                if (oldTrigger != null)
                {
                    oldTrigger.OnStateEnding?.Invoke();
                    if (oldTrigger.EndDelay > 0)
                    {
                        stateChangeTimer = Time.time + oldTrigger.EndDelay;
                        TransitioningAnimationState = newState;
                        changingFromState = true;
                        return false;
                    }
                }
            }

            if (!changingToState)
            {
                var newTrigger = onStateChange.FirstOrDefault(s => s.State == newState);
                if (newTrigger != null)
                {
                    newTrigger.OnStateStarting?.Invoke();
                    if (newTrigger.StartDelay > 0)
                    {
                        stateChangeTimer = Time.time + newTrigger.StartDelay;
                        TransitioningAnimationState = newState;
                        changingToState = true;
                        changingFromState = true;
                        return false;
                    }
                }
            }

            changingFromState = false;
            changingToState = false;
            CurrentAnimationState = newState;
            return true;
        }

        public void SetState(CharacterState newState)
        {
            CurrentState = newState;
            stateChangeTimer = Time.time + UnityEngine.Random.Range(minPatienceDuration, maxPatienceDuration);
            CurrentActivity = CharacterActivity.Idling;
            CurrentTarget = null;
        }

        private void Update()
        {
            if (changingFromState || changingToState)
            {
                if (stateChangeTimer < Time.time)
                    SetAnimationState(TransitioningAnimationState);

                return;
            }

            if (isInteractingWithPlayer)
            {
                navMeshAgent.enabled = false;
                RotateTowards(interactingWith.position);
                return;
            }

            if (stateChangeTimer < Time.time)
            {
                var currentTargetState = CurrentState;
                var nextState = agentSystem.GetRandomState(this);
                stateChangeTimer = Time.time + UnityEngine.Random.Range(minPatienceDuration, maxPatienceDuration);
                if (currentTargetState != nextState)
                    SetState(nextState);
            }

            switch (CurrentState)
            {
                case CharacterState.Idling:
                    CurrentActivity = CharacterActivity.Idling;
                    SetAnimationState(CharacterAnimationState.Idling);
                    break;
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
                            if (newTarget != null && !MoveToPoint(newTarget))
                                SetState(agentSystem.GetRandomState(this));
                        }
                        else if (CurrentActivity != CharacterActivity.Walking)
                        {
                            var distanceSqr = (transform.position - CurrentTarget.transform.position).sqrMagnitude;
                            if (distanceSqr >= CurrentTarget.StayWithinRange * CurrentTarget.StayWithinRange)
                            {
                                if (!MoveToPoint(CurrentTarget))
                                    SetState(agentSystem.GetRandomState(this));
                            }
                            else
                            {
                                CurrentActivity = CurrentState switch
                                {
                                    CharacterState.Guarding => CharacterActivity.Guarding,
                                    CharacterState.Dancing => CharacterActivity.Dancing,
                                    CharacterState.Watching => CharacterActivity.Watching,
                                    _ => CharacterActivity.Idling
                                };
                            }
                        }
                    }
                    break;
                case CharacterState.Hunting:
                    {
                        if (CurrentTarget == null || CurrentActivity != CharacterActivity.Hunting)
                        {
                            var newTarget = agentSystem.PickRandomWaypoint(InterestType.Patrol);
                            if (newTarget != null && !MoveToPoint(newTarget))
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
                        var distanceSqr = (transform.position - CurrentTarget.transform.position).sqrMagnitude;
                        if (distanceSqr <= CurrentTarget.MoveWithinRange * CurrentTarget.MoveWithinRange)
                        {
                            switch (CurrentState)
                            {
                                case CharacterState.Watching:
                                    CurrentActivity = CharacterActivity.Watching;
                                    SetAnimationState(CharacterAnimationState.Watching);
                                    break;
                                case CharacterState.Guarding:
                                    CurrentActivity = CharacterActivity.Guarding;
                                    SetAnimationState(CharacterAnimationState.Guarding);
                                    break;
                                case CharacterState.Dancing:
                                    CurrentActivity = CharacterActivity.Dancing;
                                    SetAnimationState(CharacterAnimationState.Dancing);
                                    break;
                            }
                        }
                        else
                        {
                            UpdateMovement();
                        }

                        break;
                    }
                case CharacterActivity.Idling:
                    navMeshAgent.enabled = false;
                    SetAnimationState(CharacterAnimationState.Idling);
                    break;
                default:
                    {
                        navMeshAgent.enabled = false;
                        if (CurrentTarget != null && CurrentTarget.Facing)
                        {
                            RotateTowards(CurrentTarget.transform.position);
                        }

                        SetAnimationState(CurrentState switch
                        {
                            CharacterState.Watching => CharacterAnimationState.Watching,
                            CharacterState.Guarding => CharacterAnimationState.Guarding,
                            CharacterState.Dancing => CharacterAnimationState.Dancing,
                            _ => CharacterAnimationState.Idling
                        });

                        break;
                    }
            }
        }

        void UpdateMovement()
        {
            navMeshAgent.enabled = true;

            var characterPosition = rigidBody.position;
            var characterMoveDir = navMeshAgent.steeringTarget - characterPosition;
            movementPositionInput.TargetPosition = characterPosition + characterMoveDir;
        }

        void RotateTowards(Vector3 targetPosition)
        {
            var direction = (targetPosition - transform.position).normalized;
            if (direction.sqrMagnitude < 0.001f)
                return;

            var targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z), Vector3.up);
            rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, Time.deltaTime * 5f);
        }

        bool MoveToPoint(PointOfInterest target)
        {
            CurrentTarget = null;
            navMeshAgent.enabled = true;

            if (NavMesh.SamplePosition(target.transform.position, out var hit, 10f, NavMesh.AllAreas))
            {
                CurrentTarget = target;
                navMeshAgent.SetDestination(hit.position);
                CurrentActivity = CurrentState == CharacterState.Hunting ? CharacterActivity.Hunting : CharacterActivity.Walking;
                SetAnimationState(CurrentState == CharacterState.Hunting ? CharacterAnimationState.Hunting : CharacterAnimationState.Walking);
                return true;
            }
            return false;
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

        public void ConversationStoppedCorrect()
        {
            isInteractingWithPlayer = false;
            SetAnimationState(CharacterAnimationState.GoodResponse);
            onConversationStopped.Invoke();
        }

        public void ConversationStoppedIncorrect()
        {
            isInteractingWithPlayer = false;
            SetAnimationState(CharacterAnimationState.BadResponse);
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
                isInteractingWithPlayer = true;
                interactingWith = component.transform;
                SetAnimationState(CharacterAnimationState.Talking);
                controller.StartConversation(this);
                onConversationStarted.Invoke();
            }
        }

        private void OnInteractableSelectExited(InteractableSelectExitedArgs args)
        {
        }
    }
}
