using System.Collections;
using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Cursors;
using RIEVES.GGJ2026.Core.Input;
using RIEVES.GGJ2026.Core.Interaction.Interactables;
using RIEVES.GGJ2026.Core.Interaction.Interactors;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Runtime.Characters;
using RIEVES.GGJ2026.Runtime.Controls;
using RIEVES.GGJ2026.Runtime.Decorations;
using RIEVES.GGJ2026.Runtime.Doors;
using RIEVES.GGJ2026.Runtime.Items;
using RIEVES.GGJ2026.Runtime.Movement;
using RIEVES.GGJ2026.Runtime.Popups;
using RIEVES.GGJ2026.Runtime.Resources;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace RIEVES.GGJ2026.Runtime.Player
{
    internal sealed class PlayerActor : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField]
        private Interactor interactor;

        [Header("Conversations")]
        [SerializeField]
        private ConversationController conversationController;

        [Header("Resources")]
        [SerializeField]
        private ResourceController resourceController;

        [SerializeField]
        private ItemPreview itemGrabPreview;

        [Header("Movement")]
        [SerializeField]
        private MovementController movementController;

        [Header("Cameras")]
        [SerializeField]
        private CinemachineCamera cinemachineCamera;

        [SerializeField]
        [Min(0f)]
        private float zoomInSpeed = 8f;

        [SerializeField]
        [Min(0f)]
        private float zoomInFov = 20f;

        [Header("Inputs")]
        [SerializeField]
        private InputActionReference interactInputAction;

        [SerializeField]
        private InputActionReference zoomInputListener;

        [Header("UI")]
        [SerializeField]
        private HoverPopupViewController defaultHoverPopupController;

        [SerializeField]
        private HoverPopupViewController characterHoverPopupController;

        [SerializeField]
        private HoverPopupViewController itemHoverPopupController;

        [SerializeField]
        private HoverPopupViewController doorHoverPopupController;

        [SerializeField]
        private ControlsViewController controlsViewController;

        [Header("Gameplay")]
        [SerializeField]
        [Min(0f)]
        private float loseTickDelay = 0.3f;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onItemUsed;

        [SerializeField]
        private UnityEvent onDecorationPunched;

        private float initialFov;
        private float currentFov;
        private float targetFov;

        private ICursorSystem cursorSystem;
        private IInputSystem inputSystem;
        private ISceneSystem sceneSystem;

        private void Awake()
        {
            cursorSystem = GameManager.GetSystem<ICursorSystem>();
            inputSystem = GameManager.GetSystem<IInputSystem>();
            sceneSystem = GameManager.GetSystem<ISceneSystem>();
        }

        private void Start()
        {
            initialFov = cinemachineCamera.Lens.FieldOfView;
            targetFov = cinemachineCamera.Lens.FieldOfView;
            currentFov = cinemachineCamera.Lens.FieldOfView;

            // interactor.HoverValidator = IsInteractableValid;
            interactor.SelectValidator = IsInteractableValid;

            cursorSystem.LockCursor();
        }

        private void OnEnable()
        {
            resourceController.OnAlcoholChanged += OnAlcoholChanged;

            movementController.OnMoveEntered += OnMoveEntered;

            conversationController.OnConversationStarted += OnConversationStarted;
            conversationController.OnConversationStopped += OnConversationStopped;

            interactor.OnHoverEntered += OnInteractorHoverEntered;
            interactor.OnHoverExited += OnInteractorHoverExited;
            interactor.OnSelectEntered += OnInteractorSelectEntered;
            interactor.OnSelectExited += OnInteractorSelectExited;

            interactInputAction.action.performed += OnInteractPerformed;
            interactInputAction.action.canceled += OnInteractCanceled;

            zoomInputListener.action.performed += OnZoomPerformed;
            zoomInputListener.action.canceled += OnZoomCanceled;
        }

        private void OnDisable()
        {
            resourceController.OnAlcoholChanged -= OnAlcoholChanged;

            movementController.OnMoveEntered -= OnMoveEntered;

            conversationController.OnConversationStarted -= OnConversationStarted;
            conversationController.OnConversationStopped -= OnConversationStopped;

            interactor.OnHoverEntered -= OnInteractorHoverEntered;
            interactor.OnHoverExited -= OnInteractorHoverExited;
            interactor.OnSelectEntered -= OnInteractorSelectEntered;
            interactor.OnSelectExited -= OnInteractorSelectExited;

            interactInputAction.action.performed -= OnInteractPerformed;
            interactInputAction.action.canceled -= OnInteractCanceled;

            zoomInputListener.action.performed -= OnZoomPerformed;
            zoomInputListener.action.canceled -= OnZoomCanceled;
        }

        private void OnDestroy()
        {
            cursorSystem.UnLockCursor();
        }

        private void Update()
        {
            UpdateCameraZoom();
        }

        private void OnAlcoholChanged(AlcoholChangedArgs args)
        {
            if (args.Ratio <= 0f)
            {
                StartCoroutine(LoseGameRoutine());
                return;
            }

            if (args.Ratio >= 1f)
            {
                sceneSystem.LoadGameVictoryScene();
            }
        }

        private void OnMoveEntered()
        {
            controlsViewController.HideView();
        }

        private void OnConversationStarted()
        {
            cursorSystem.UnLockCursor();
            inputSystem.DisablePlayerInput();
            interactor.enabled = false;
        }

        private void OnConversationStopped()
        {
            cursorSystem.LockCursor();
            inputSystem.EnablePlayerInput();
            interactor.enabled = true;
        }

        private void OnInteractorHoverEntered(InteractorHoverEnteredArgs args)
        {
            if (args.Interactable is not Component component)
            {
                defaultHoverPopupController.TitleText = args.Interactable.Name;
                defaultHoverPopupController.ShowView();
                return;
            }

            var character = component.GetComponentInParent<CharacterActor>();
            if (character)
            {
                characterHoverPopupController.TitleText = character.CharacterData.CharacterName;
                characterHoverPopupController.IsBlocked = conversationController.IsContainsAnyMessages(character) == false;
                characterHoverPopupController.ShowView();
                return;
            }

            var item = component.GetComponentInParent<ItemActor>();
            if (item)
            {
                itemHoverPopupController.TitleText = item.Data.ItemName;
                itemHoverPopupController.ShowView();
                return;
            }

            var door = component.GetComponentInParent<DoorActor>();
            if (door)
            {
                doorHoverPopupController.TitleText = door.IsOpen ? "Uždaryti" : "Atidaryti";
                doorHoverPopupController.ShowView();
            }
        }

        private void OnInteractorHoverExited(InteractorHoverExitedArgs args)
        {
            characterHoverPopupController.HideView();
            itemHoverPopupController.HideView();
            defaultHoverPopupController.HideView();
            doorHoverPopupController.HideView();
        }

        private void OnInteractorSelectEntered(InteractorSelectEnteredArgs args)
        {
            if (args.Interactable is not Component component)
            {
                return;
            }

            args.Interactable.Deselect();

            var character = component.GetComponentInParent<CharacterActor>();
            if (character)
            {
                conversationController.StartConversation(character);
                character.StartConversation(transform);
                return;
            }

            var item = component.GetComponentInParent<ItemActor>();
            if (item)
            {
                itemGrabPreview.Show(item.Data);
                resourceController.AddAlcohol(item.Data.Value);
                item.Use();
                onItemUsed.Invoke();
                return;
            }

            var decoration = component.GetComponentInParent<DecorationActor>();
            if (decoration)
            {
                decoration.Punch();
                onDecorationPunched.Invoke();
                return;
            }

            var door = component.GetComponentInParent<DoorActor>();
            if (door)
            {
                door.Open();
            }
        }

        private void OnInteractorSelectExited(InteractorSelectExitedArgs args)
        {
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (interactor.IsHovering)
            {
                interactor.Select();
            }
        }

        private void OnInteractCanceled(InputAction.CallbackContext context)
        {
        }


        private void OnZoomPerformed(InputAction.CallbackContext context)
        {
            targetFov = zoomInFov;
        }

        private void OnZoomCanceled(InputAction.CallbackContext context)
        {
            targetFov = initialFov;
        }

        private void UpdateCameraZoom()
        {
            currentFov = Mathf.Lerp(
                currentFov,
                targetFov,
                Time.deltaTime * zoomInSpeed
            );

            cinemachineCamera.Lens.FieldOfView = currentFov;
        }

        private IEnumerator LoseGameRoutine()
        {
            yield return new WaitForSeconds(loseTickDelay);
            sceneSystem.LoadGameOverScene();
        }

        private bool IsInteractableValid(IInteractable interactable)
        {
            if (interactable is not Component component)
            {
                return true;
            }

            var character = component.GetComponentInParent<CharacterActor>();
            if (character)
            {
                return conversationController.IsContainsAnyMessages(character);
            }

            return true;
        }
    }
}
