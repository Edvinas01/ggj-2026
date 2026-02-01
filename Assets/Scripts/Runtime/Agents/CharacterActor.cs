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

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Application.isPlaying == false && data)
            {
                Initialize(data);
            }
        }
#endif

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
        float animationDelayTimer = -100f;
        bool isInteractingWithPlayer = false;

        public Queue<Func<bool>> CallBacks = new Queue<Func<bool>>();

        public void SetAnimationState(CharacterAnimationState newState)
        {
            // We only want to keep the ending callback. This handles interrupting animations properly.
            while (CallBacks.Count > 1)
            {
                CallBacks.Dequeue().Invoke();
            }

            // If the state is the same, do nothing.
            if (newState == CurrentAnimationState)
                return;

            var newTrigger = onStateChange.FirstOrDefault(s => s.State == newState);
            if (newTrigger != null)
            {
                CallBacks.Enqueue(() =>
                {
                    newTrigger.OnStateStarting?.Invoke();
                    animationDelayTimer = Time.time + newTrigger.StartDelay;
                    CurrentAnimationState = newState;
                    return newTrigger.StartDelay > 0;
                });
            }

            CallBacks.Enqueue(() =>
            {
                CurrentAnimationState = newState;
                animationDelayTimer = Time.time;
                return false;
            });

            if (newTrigger != null)
            {
                CallBacks.Enqueue(() =>
                {
                    newTrigger.OnStateEnding?.Invoke();
                    animationDelayTimer = Time.time + newTrigger.EndDelay;
                    return newTrigger.EndDelay > 0;
                });
            }
            else
            {
                CallBacks.Enqueue(() => false);
            }

            while (CallBacks.Count > 1)
            {
                var callback = CallBacks.Dequeue();
                if (callback.Invoke())
                    return;
            }

            return;
        }

        public void SetPatienceDuration(CharacterState state)
        {
            var agentPrefs = CharacterData.ActivityPatience;
            var matchingPref = agentPrefs.FirstOrDefault(p => p.activity == state);
            if (matchingPref.minTime > 0 && matchingPref.maxTime >= matchingPref.minTime)
            {
                stateChangeTimer = Time.time + UnityEngine.Random.Range(matchingPref.minTime, matchingPref.maxTime);
            }
            else
            {
                stateChangeTimer = Time.time + UnityEngine.Random.Range(15f, 25f);
            }
        }

        public void SetState(CharacterState newState)
        {
            CurrentState = newState;
            SetPatienceDuration(newState);
            CurrentActivity = CharacterActivity.Idling;
            CurrentTarget = null;
        }

        private void Update()
        {
            // Leave out the last callback to be played on interruption.
            while (CallBacks.Count > 1)
            {
                bool blocking = true;
                if (animationDelayTimer <= Time.time)
                    blocking = CallBacks.Dequeue().Invoke();

                if (blocking)
                    return;
            }

            if (isInteractingWithPlayer)
            {
                RotateTowards(interactingWith.position);
                return;
            }

            if (stateChangeTimer <= Time.time)
            {
                var currentTargetState = CurrentState;
                var nextState = agentSystem.GetRandomState(this);
                SetPatienceDuration(nextState);
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
                    SetAnimationState(CharacterAnimationState.Idling);
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
                            if (newTarget != null && !StartMovement(newTarget))
                                SetState(agentSystem.GetRandomState(this));
                        }
                        else if (CurrentActivity != CharacterActivity.Walking)
                        {
                            var distanceSqr = (transform.position - CurrentTarget.transform.position).sqrMagnitude;
                            if (distanceSqr >= CurrentTarget.StayWithinRange * CurrentTarget.StayWithinRange)
                            {
                                if (!StartMovement(CurrentTarget))
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
                        SetAnimationState(CharacterAnimationState.Hunting);
                        if (CurrentTarget == null || CurrentActivity != CharacterActivity.Hunting)
                        {
                            var newTarget = agentSystem.PickRandomWaypoint(InterestType.Patrol);
                            if (newTarget != null && !StartMovement(newTarget))
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
                                    StopMovement();
                                    break;
                                case CharacterState.Guarding:
                                    CurrentActivity = CharacterActivity.Guarding;
                                    SetAnimationState(CharacterAnimationState.Guarding);
                                    StopMovement();
                                    break;
                                case CharacterState.Dancing:
                                    CurrentActivity = CharacterActivity.Dancing;
                                    SetAnimationState(CharacterAnimationState.Dancing);
                                    StopMovement();
                                    break;
                                case CharacterState.Hunting:
                                    var newTarget = agentSystem.PickRandomWaypoint(InterestType.Patrol);
                                    if (newTarget != null && !StartMovement(newTarget))
                                        SetState(agentSystem.GetRandomState(this));
                                    break;
                            }
                        }
                        else
                        {
                            SetAnimationState(CurrentState == CharacterState.Hunting ? CharacterAnimationState.Hunting : CharacterAnimationState.Walking);
                            UpdateMovement();
                        }

                        break;
                    }
                case CharacterActivity.Idling:
                    SetAnimationState(CharacterAnimationState.Idling);
                    break;
                default:
                    {
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

        bool StartMovement(PointOfInterest target)
        {
            if (target == null)
            {
                SetAnimationState(CharacterAnimationState.Idling);
                StopMovement();
                return false;
            }

            CurrentTarget = null;
            navMeshAgent.enabled = true;

            if (NavMesh.SamplePosition(target.transform.position, out var hit, 10f, NavMesh.AllAreas))
            {
                CurrentTarget = target;
                navMeshAgent.SetDestination(hit.position);

                var isHunting = CurrentState == CharacterState.Hunting;
                CurrentActivity = isHunting ? CharacterActivity.Hunting : CharacterActivity.Walking;
                SetAnimationState(isHunting ? CharacterAnimationState.Hunting : CharacterAnimationState.Walking);
                return true;
            }

            SetAnimationState(CharacterAnimationState.Idling);
            StopMovement();
            return false;
        }

        void UpdateMovement()
        {
            navMeshAgent.enabled = true;
            var characterPosition = rigidBody.position;
            var characterMoveDir = navMeshAgent.steeringTarget - characterPosition;
            movementPositionInput.TargetPosition = characterPosition + characterMoveDir;
        }

        void StopMovement()
        {
            navMeshAgent.enabled = false;
            movementPositionInput.ClearPosition();
        }

        private void LateUpdate()
        {
            navMeshAgent.nextPosition = rigidBody.position;
        }

        void RotateTowards(Vector3 targetPosition)
        {
            var direction = (targetPosition - transform.position).normalized;
            if ((targetPosition - transform.position).sqrMagnitude < 0.001f)
                return;

            var targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z), Vector3.up);
            rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, Time.deltaTime * 5f);
        }

        public void Initialize(CharacterData newData)
        {
            if (runtimeData)
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    DestroyImmediate(runtimeData);
                }
                else
                {
                    Destroy(runtimeData);
                }
#else
                Destroy(runtimeData);
#endif
            }

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

        bool wasMovingBeforeConversation = false;

        public void ConversationStarted()
        {
            isInteractingWithPlayer = true;
            wasMovingBeforeConversation = navMeshAgent.enabled;
            if (wasMovingBeforeConversation)
                StopMovement();

            SetAnimationState(CharacterAnimationState.Talking);
            onConversationStarted.Invoke();
        }

        public void ConversationStoppedCorrect()
        {
            isInteractingWithPlayer = false;
            if (wasMovingBeforeConversation)
                StartMovement(CurrentTarget);

            SetAnimationState(CharacterAnimationState.GoodResponse);
            onConversationStopped.Invoke();
        }

        public void ConversationStoppedIncorrect()
        {
            isInteractingWithPlayer = false;
            if (wasMovingBeforeConversation)
                StartMovement(CurrentTarget);

            SetAnimationState(CharacterAnimationState.BadResponse);
            onConversationStopped.Invoke();
        }

        public void ConversationStoppedNeutral()
        {
            isInteractingWithPlayer = false;
            if (wasMovingBeforeConversation)
                StartMovement(CurrentTarget);

            SetAnimationState(CharacterAnimationState.NeutralResponse);
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
                interactingWith = component.transform;
                controller.StartConversation(this);
                ConversationStarted();
            }
        }

        private void OnInteractableSelectExited(InteractableSelectExitedArgs args)
        {
        }
    }
}
