using RIEVES.GGJ2026.Core.Interaction.Interactables;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Items
{
    internal sealed class ItemActor : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private ItemData data;

        [SerializeField]
        private Interactable interactable;

        public string Name => name; // TODO: use item name

        private void OnEnable()
        {
            interactable.OnHoverEntered += OnInteractableHoverEntered;
            interactable.OnHoverExited += OnInteractableHoverExited;
            interactable.OnSelectEntered += OnInteractableSelectEntered;
            interactable.OnSelectExited += OnInteractableSelectExited;
        }

        private void OnDisable()
        {
            interactable.OnHoverEntered -= OnInteractableHoverEntered;
            interactable.OnHoverExited -= OnInteractableHoverExited;
            interactable.OnSelectEntered -= OnInteractableSelectEntered;
            interactable.OnSelectExited -= OnInteractableSelectExited;
        }

        private void OnInteractableHoverEntered(InteractableHoverEnteredArgs args)
        {
            Debug.Log($"Item hover entered {name}", this);
        }

        private void OnInteractableHoverExited(InteractableHoverExitedArgs args)
        {
            Debug.Log($"Item hover exited {name}", this);
        }

        private void OnInteractableSelectEntered(InteractableSelectEnteredArgs args)
        {
            Debug.Log($"Item select entered {name}", this);
        }

        private void OnInteractableSelectExited(InteractableSelectExitedArgs args)
        {
            Debug.Log($"Item select exited {name}", this);
        }
    }
}
