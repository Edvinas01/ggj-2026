using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Cursors;
using RIEVES.GGJ2026.Core.Input;
using RIEVES.GGJ2026.Core.Interaction.Interactors;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Runtime.Characters;
using RIEVES.GGJ2026.Runtime.Items;
using RIEVES.GGJ2026.Runtime.Movement;
using RIEVES.GGJ2026.Runtime.Popups;
using RIEVES.GGJ2026.Runtime.Resources;
using UnityEngine;
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

        [Header("Movement")]
        [SerializeField]
        private MovementController movementController;

        [Header("Inputs")]
        [SerializeField]
        private InputActionReference interactInputAction;

        [Header("UI")]
        [SerializeField]
        private HoverPopupViewController defaultHoverPopupController;

        [SerializeField]
        private HoverPopupViewController characterHoverPopupController;

        [SerializeField]
        private HoverPopupViewController itemHoverPopupController;

        private ICursorSystem cursorSystem;
        private IInputSystem inputSystem;
        private ISceneSystem sceneSystem;

        private void Awake()
        {
            cursorSystem = GameManager.GetSystem<ICursorSystem>();
            inputSystem = GameManager.GetSystem<IInputSystem>();
            sceneSystem = GameManager.GetSystem<ISceneSystem>();
        }

        private void OnEnable()
        {
            resourceController.OnAlcoholChanged += OnAlcoholChanged;

            conversationController.OnConversationStarted += OnConversationStarted;
            conversationController.OnConversationStopped += OnConversationStopped;

            interactor.OnHoverEntered += OnInteractorHoverEntered;
            interactor.OnHoverExited += OnInteractorHoverExited;
            interactor.OnSelectEntered += OnInteractorSelectEntered;
            interactor.OnSelectExited += OnInteractorSelectExited;

            interactInputAction.action.performed += OnInteractPerformed;
            interactInputAction.action.canceled += OnInteractCanceled;
        }

        private void OnDisable()
        {
            resourceController.OnAlcoholChanged -= OnAlcoholChanged;

            conversationController.OnConversationStarted -= OnConversationStarted;
            conversationController.OnConversationStopped -= OnConversationStopped;

            interactor.OnHoverEntered -= OnInteractorHoverEntered;
            interactor.OnHoverExited -= OnInteractorHoverExited;
            interactor.OnSelectEntered -= OnInteractorSelectEntered;
            interactor.OnSelectExited -= OnInteractorSelectExited;

            interactInputAction.action.performed -= OnInteractPerformed;
            interactInputAction.action.canceled -= OnInteractCanceled;
        }

        private void Start()
        {
            cursorSystem.LockCursor();
        }

        private void OnDestroy()
        {
            cursorSystem.UnLockCursor();
        }

        private void OnAlcoholChanged(AlcoholChangedArgs args)
        {
            if (args.Ratio <= 0f)
            {
                sceneSystem.LoadGameOverScene();
                return;
            }

            if (args.Ratio >= 1f)
            {
                sceneSystem.LoadGameVictoryScene();
            }
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
                characterHoverPopupController.ShowView();
                return;
            }

            var item = component.GetComponentInParent<ItemActor>();
            if (item)
            {
                itemHoverPopupController.TitleText = item.Name;
                itemHoverPopupController.ShowView();
            }
        }

        private void OnInteractorHoverExited(InteractorHoverExitedArgs args)
        {
            characterHoverPopupController.HideView();
            itemHoverPopupController.HideView();
            defaultHoverPopupController.HideView();
        }

        private void OnInteractorSelectEntered(InteractorSelectEnteredArgs args)
        {
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
    }
}
