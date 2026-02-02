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

        public ItemData Data => data;

        private void Start()
        {
            if (data)
            {
                Initialize(data);
            }
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

        public void Initialize(ItemData newData)
        {
            data = newData;

            var block = new MaterialPropertyBlock();

            block.SetTexture(texturePropertyId, newData.Texture);
            frontRenderer.SetPropertyBlock(block);
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
