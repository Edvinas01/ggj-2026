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

        private readonly List<TriggerVolume> enteredVolumes;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var volume = other.GetComponentInParent<TriggerVolume>();
            if (volume && enteredVolumes.Contains(volume) == false)
            {
                enteredVolumes.Add(volume);
                volume.TriggerEnter();
                onEntered.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var volume = other.GetComponentInParent<TriggerVolume>();
            if (volume && enteredVolumes.Remove(volume))
            {
                volume.TriggerExit();
                onExited.Invoke();
            }
        }
    }
}
