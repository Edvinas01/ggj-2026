using RIEVES.GGJ2026.Core.Constants;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Interaction.Interactors
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(GrabInteractorSettings),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Interaction/Grab Interactor Settings",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    internal sealed class GrabInteractorSettings : ScriptableObject, IRaycastInteractorSettings
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Hovering", Expanded = true)]
        [Sirenix.OdinInspector.PropertyRange(0f, 100f)]
#else
        [Header("Hovering")]
        [Range(0f, 100f)]
#endif
        [SerializeField]
        private float interactableFollowSpeed = 3f;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Raycast", Expanded = true)]
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#else
        [Header("Raycast")]
#endif
        [SerializeField]
        private RaycastInteractorData data;

        public float InteractableFollowSpeed => interactableFollowSpeed;

        public float RaycastDistance => data.RaycastDistance;

        public float RaycastRadius => data.RaycastRadius;

        public LayerMask RaycastLayer => data.RaycastLayer;

        public QueryTriggerInteraction QueryTriggerInteraction => data.QueryTriggerInteraction;

        public Color RaycastColor => data.RaycastColor;
    }
}
