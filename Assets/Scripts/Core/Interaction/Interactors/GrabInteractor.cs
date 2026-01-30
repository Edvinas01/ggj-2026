using RIEVES.GGJ2026.Core.Interaction.Interactables;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Interaction.Interactors
{
    internal sealed class GrabInteractor : RaycastInteractor
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
        [Sirenix.OdinInspector.Required]
#else
        [Header("General")]
#endif
        [SerializeField]
        private GrabInteractorSettings settings;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Transforms", Expanded = true)]
        [Sirenix.OdinInspector.Required]
#endif
        [SerializeField]
        private Transform grabTransform;

        protected override IRaycastInteractorSettings Settings => settings;

        protected override bool IsValid(IInteractable interactable)
        {
            return interactable is GrabInteractable;
        }

        protected override void OnLateUpdated()
        {
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            if (grabTransform == false)
            {
                return;
            }

            if (IsSelecting == false)
            {
                return;
            }

            var targetPosition = grabTransform.position;
            var targetRotation = grabTransform.rotation;
            var speed = Time.deltaTime * settings.InteractableFollowSpeed;

            foreach (var selectedInteractable in SelectedInteractables)
            {
                var currentPosition = selectedInteractable.Position;
                selectedInteractable.Position = Vector3.Lerp(
                    currentPosition,
                    targetPosition,
                    speed
                );

                var currentRotation = selectedInteractable.Rotation;
                selectedInteractable.Rotation = Quaternion.Lerp(
                    currentRotation,
                    targetRotation,
                    speed
                );
            }
        }
    }
}
