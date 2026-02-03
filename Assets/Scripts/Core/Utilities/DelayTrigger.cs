using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Utilities
{
    internal sealed class DelayTrigger : MonoBehaviour
    {
        [Min(0f)]
        [SerializeField]
        private float delaySeconds = 3f;

        [SerializeField]
        private UnityEvent onTrigger;

        private bool isTriggered;

        public void Trigger()
        {
            StartCoroutine(TriggerRoutine());
        }

        private IEnumerator TriggerRoutine()
        {
            if (isTriggered)
            {
                yield break;
            }

            isTriggered = true;
            yield return new WaitForSeconds(delaySeconds);
            onTrigger.Invoke();
            Destroy(this);
        }
    }
}
