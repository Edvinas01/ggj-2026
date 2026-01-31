using RIEVES.GGJ2026.Core.Interaction.Interactables;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Interaction.Interactors
{
    internal sealed class SimpleRaycastInteractor : RaycastInteractor
    {
        [SerializeField]
        private SimpleRaycastInteractorSettings settings;

        protected override IRaycastInteractorSettings Settings => settings;

        protected override bool IsValid(IInteractable interactable)
        {
            return true;
        }
    }
}
