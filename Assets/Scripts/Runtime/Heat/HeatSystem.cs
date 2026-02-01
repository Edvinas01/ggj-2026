using CHARK.GameManagement.Systems;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        [ParamRef]
        [SerializeField]
        private string fmodParameterId;

        public float CurrentHeat { get; private set; } = 1f;

        public void OnUpdated(float deltaTime)
        {
            CurrentHeat += heatPerSecond * heatMultiplier * deltaTime;
            RuntimeManager.StudioSystem.setParameterByName(fmodParameterId, CurrentHeat);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CurrentHeat = 1f;
        }
    }
}
