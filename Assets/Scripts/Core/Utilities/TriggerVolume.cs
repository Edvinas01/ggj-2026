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

        public void TriggerEnter()
        {
            onEntered.Invoke();
        }

        public void TriggerExit()
        {
            onExited.Invoke();
        }
    }
}
