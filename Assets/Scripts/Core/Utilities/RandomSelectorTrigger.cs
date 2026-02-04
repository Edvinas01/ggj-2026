using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Utilities
{
    internal sealed class RandomSelectorTrigger : MonoBehaviour
    {
        [Serializable]
        private sealed class TriggerConfig
        {
            [SerializeField]
            public UnityEvent onTriggered;
        }

        [SerializeField]
        private List<TriggerConfig> triggers;

        public void Trigger()
        {
            if (triggers.TryGetRandom(out var trigger))
            {
                trigger.onTriggered.Invoke();
            }
        }
    }
}
