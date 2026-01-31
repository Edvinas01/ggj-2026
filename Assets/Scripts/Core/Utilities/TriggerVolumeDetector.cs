using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Utilities
{
    [RequireComponent(typeof(Collider))]
    internal sealed class TriggerVolumeDetector : MonoBehaviour
    {
        private Collider triggerCollider;

        [SerializeField]
        private UnityEvent onEntered;

        [SerializeField]
        private UnityEvent onExited;

        private readonly Dictionary<TriggerVolume, int> enteredVolumes = new();

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var volume = other.GetComponentInParent<TriggerVolume>();
            if (volume == false)
            {
                return;
            }

            if (enteredVolumes.TryGetValue(volume, out var count))
            {
                enteredVolumes[volume] = count + 1;
            }
            else
            {
                enteredVolumes.Add(volume, 1);

                volume.TriggerEnter();
                onEntered.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var volume = other.GetComponentInParent<TriggerVolume>();
            if (volume == false)
            {
                return;
            }

            if (enteredVolumes.TryGetValue(volume, out var count) == false)
            {
                return;
            }

            enteredVolumes[volume] = count - 1;
            if (enteredVolumes[volume] <= 0)
            {
                enteredVolumes.Remove(volume);

                volume.TriggerExit();
                onExited.Invoke();
            }
        }
    }
}
