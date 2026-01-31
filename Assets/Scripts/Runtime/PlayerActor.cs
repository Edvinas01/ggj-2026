using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Cursors;
using RIEVES.GGJ2026.Core.Interaction.Interactors;
using RIEVES.GGJ2026.Runtime.Movement;
using RIEVES.GGJ2026.Runtime.Popups;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime
{
    internal sealed class PlayerActor : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        private HoverPopupViewController hoverPopupController;

        [Header("Interaction")]
        [SerializeField]
        private Interactor interactor;

        [Header("Movement")]
        [SerializeField]
        private MovementController movementController;

        private ICursorSystem cursorSystem;

        public MovementController MovementController => movementController;

        private void Awake()
        {
            cursorSystem = GameManager.GetSystem<ICursorSystem>();
        }

        private void OnEnable()
        {
            interactor.OnHoverEntered += OnInteractorHoverEntered;
            interactor.OnHoverExited += OnInteractorHoverExited;
            interactor.OnSelectEntered += OnInteractorSelectEntered;
            interactor.OnSelectExited += OnInteractorSelectExited;
        }

        private void OnDisable()
        {
            interactor.OnHoverEntered -= OnInteractorHoverEntered;
            interactor.OnHoverExited -= OnInteractorHoverExited;
            interactor.OnSelectEntered -= OnInteractorSelectEntered;
            interactor.OnSelectExited -= OnInteractorSelectExited;
        }

        private void Start()
        {
            cursorSystem.LockCursor();
        }

        private void OnInteractorHoverEntered(InteractorHoverEnteredArgs args)
        {
            hoverPopupController.TitleText = args.Interactable.Name;
            hoverPopupController.ShowView();
        }

        private void OnInteractorHoverExited(InteractorHoverExitedArgs args)
        {
            hoverPopupController.HideView();
        }

        private void OnInteractorSelectEntered(InteractorSelectEnteredArgs args)
        {
            Debug.Log($"Player select entered {args.Interactable.Name}", this);
        }

        private void OnInteractorSelectExited(InteractorSelectExitedArgs args)
        {
            Debug.Log($"Player select exited {args.Interactable.Name}", this);
        }

        private void OnDestroy()
        {
            cursorSystem.UnLockCursor();
        }
    }
}
