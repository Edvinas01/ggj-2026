using RIEVES.GGJ2026.Core.Interaction.Interactables;
using RIEVES.GGJ2026.Runtime.Resources;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Items
{
    internal sealed class ItemActor : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private ItemData data;

        [Header("Interaction")]
        [SerializeField]
        private Interactable interactable;

        [Header("Rendering")]
        [SerializeField]
        private Renderer frontRenderer;

        [SerializeField]
        private string texturePropertyId = "_BaseMap";

        public string Name => data.ItemName;

        private void Start()
        {
            var block = new MaterialPropertyBlock();

            block.SetTexture(texturePropertyId, data.Texture);
            frontRenderer.SetPropertyBlock(block);
        }

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

            var resources = component.GetComponentInParent<ResourceController>();
            if (resources)
            {
                resources.AddAlcohol(data.Value);
            }

            Destroy(gameObject);
        }

        private void OnInteractableSelectExited(InteractableSelectExitedArgs args)
        {
        }
    }
}
