using UnityEngine;
using UnityEngine.Rendering;

namespace RIEVES.GGJ2026.Core.Utilities
{
    [RequireComponent(typeof(ReflectionProbe))]
    internal sealed class ReflectionProbeUpdater : MonoBehaviour
    {
        [Range(0f, 16f)]
        [SerializeField]
        private float updateIntervalSeconds = 0.1f;

        private ReflectionProbe probe;
        private float timer;

        private void Awake()
        {
            probe = GetComponent<ReflectionProbe>();
            probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;

            timer = updateIntervalSeconds;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                probe.RenderProbe();
                timer = updateIntervalSeconds;
            }
        }
    }
}
