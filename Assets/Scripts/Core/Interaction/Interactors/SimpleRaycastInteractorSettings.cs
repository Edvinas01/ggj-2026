using RIEVES.GGJ2026.Core.Constants;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Interaction.Interactors
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(SimpleRaycastInteractorSettings),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Interaction/Simple Raycast Interactor Settings",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    internal sealed class SimpleRaycastInteractorSettings : ScriptableObject, IRaycastInteractorSettings
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Raycast", Expanded = true)]
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#else
        [Header("Raycast")]
#endif
        [SerializeField]
        private RaycastInteractorData data;

        public float RaycastDistance => data.RaycastDistance;

        public float RaycastRadius => data.RaycastRadius;

        public LayerMask RaycastLayer => data.RaycastLayer;

        public QueryTriggerInteraction QueryTriggerInteraction => data.QueryTriggerInteraction;

        public Color RaycastColor => data.RaycastColor;
    }
}
