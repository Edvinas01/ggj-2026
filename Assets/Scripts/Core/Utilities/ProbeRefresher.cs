using UnityEngine;
using UnityEngine.Rendering;

namespace RIEVES.GGJ2026.Core.Utilities
{
    [RequireComponent(typeof(ReflectionProbe))]
    [RequireComponent(typeof(Collider))]
    internal sealed class ProbeRefresher : MonoBehaviour
    {
        [Range(0f, 16f)]
        [SerializeField]
        private float refreshIntervalSeconds = 0.1f;

        private Collider probeRadiusCollider;
        private ReflectionProbe probe;

        private bool isVisible;
        private float timer;

        private void Awake()
        {
            probeRadiusCollider = GetComponent<Collider>();
            probeRadiusCollider.isTrigger = true;

            probe = GetComponent<ReflectionProbe>();
            probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;

            timer = refreshIntervalSeconds;
        }

        private void Update()
        {
            if (isVisible == false)
            {
                return;
            }

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                probe.RenderProbe();
                timer = refreshIntervalSeconds;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            isVisible = true;
        }

        private void OnTriggerExit(Collider other)
        {
            isVisible = false;
        }
    }
}
