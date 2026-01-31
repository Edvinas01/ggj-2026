using RIEVES.GGJ2026.Core.Interaction.Interactables;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Interaction.Interactors
{
    public abstract class RaycastInteractor : Interactor
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Transforms", Expanded = true)]
        [Sirenix.OdinInspector.Required]
#else
        [Header("General")]
#endif
        [SerializeField]
        private Transform raycastTransform;

        private RaycastHit raycastHit;

        protected abstract IRaycastInteractorSettings Settings { get; }

        protected abstract bool IsValid(IInteractable interactable);

        private Transform InteractorTransform
        {
            get
            {
                if (raycastTransform)
                {
                    return raycastTransform;
                }

                return transform;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Settings == null)
            {
                return;
            }

            Gizmos.color = Settings.RaycastColor;

            var interactorTransform = InteractorTransform;
            var interactorPosition = interactorTransform.position;
            var interactorForward = interactorTransform.forward;

            if (raycastHit.collider)
            {
                Gizmos.DrawLine(
                    interactorPosition,
                    raycastHit.point
                );

                Gizmos.DrawWireSphere(
                    raycastHit.point,
                    Settings.RaycastRadius
                );
            }
            else
            {
                Gizmos.DrawRay(
                    interactorPosition,
                    interactorForward * Settings.RaycastDistance
                );

                Gizmos.DrawWireSphere(
                    interactorPosition + interactorTransform.forward * Settings.RaycastDistance,
                    Settings.RaycastRadius
                );
            }
        }
#endif

        protected override void OnPhysicsUpdated()
        {
            base.OnPhysicsUpdated();
            UpdateHovering();
        }

        private void UpdateHovering()
        {
            if (IsSelecting)
            {
                // Already selected something - busy.
                return;
            }

            if (TryRaycast(out raycastHit) == false)
            {
                // Didn't hit anything - we're not hovering any objects.
                UnHover();
                return;
            }

            var hitCollider = raycastHit.collider;
            if (hitCollider == false)
            {
                // No collider, destroyed object?
                UnHover();
                return;
            }

            var interactable = hitCollider.GetComponentInParent<IInteractable>();
            var isValid = interactable is { IsEnabled: true } && IsValid(interactable);
            if (isValid == false)
            {
                // We hit something, but that something is incorrect.
                UnHover();
                return;
            }

            if (IsHovered(interactable))
            {
                return;
            }

            // Only one interactable should be hovered at any given time.
            UnHover();

            Hover(interactable);
        }

        private bool TryRaycast(out RaycastHit hit)
        {
            var interactorTransform = InteractorTransform;

            var isHit = Physics.SphereCast(
                interactorTransform.position,
                Settings.RaycastRadius,
                interactorTransform.forward * Settings.RaycastDistance,
                out hit,
                Settings.RaycastDistance,
                Settings.RaycastLayer,
                Settings.QueryTriggerInteraction
            );

            return isHit;
        }
    }
}
