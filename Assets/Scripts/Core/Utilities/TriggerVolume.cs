using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Utilities
{
    internal sealed class TriggerVolume : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onEntered;

        [SerializeField]
        private UnityEvent onExited;

        public void TriggerExit()
        {
            onEntered.Invoke();
        }

        public void TriggerEnter()
        {
            onExited.Invoke();
        }
    }
}
