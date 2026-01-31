using CHARK.GameManagement.Systems;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Heat
{
    internal sealed class HeatSystem : MonoSystem, IUpdateListener
    {
        [Min(0f)]
        [SerializeField]
        private float heatPerSecond = 0.01f;

        [Min(0f)]
        [SerializeField]
        private float heatMultiplier = 1f;

        [Min(0f)]
        [SerializeField]
        private float currentHeat = 1f;

        public float CurrentHeat => currentHeat;

        public void OnUpdated(float deltaTime)
        {
            currentHeat += heatPerSecond * heatMultiplier * deltaTime;
        }
    }
}
