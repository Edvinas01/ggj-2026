using UnityEngine;

namespace RIEVES.GGJ2026.Core.Interaction.Interactors
{
    internal interface IRaycastInteractorSettings
    {
        public float RaycastDistance { get; }

        public float RaycastRadius { get; }

        public LayerMask RaycastLayer { get; }

        public QueryTriggerInteraction QueryTriggerInteraction { get; }

        public Color RaycastColor { get; }
    }
}
