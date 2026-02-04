using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Utilities
{
    internal sealed class DelayTrigger : MonoBehaviour
    {
        [Header("Features")]
        [Min(0f)]
        [SerializeField]
        private float delaySeconds = 3f;

        [SerializeField]
        private bool isTriggerOnce = true;

        [SerializeField]
        private bool isDestroyOnTrigger = true;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onTrigger;

        private bool isTriggered;

        public void Trigger()
        {
            StartCoroutine(TriggerRoutine());
        }

        private IEnumerator TriggerRoutine()
        {
            if (isTriggerOnce && isTriggered)
            {
                yield break;
            }

            isTriggered = true;
            yield return new WaitForSeconds(delaySeconds);
            onTrigger.Invoke();

            if (isDestroyOnTrigger)
            {
                Destroy(this);
            }
        }
    }
}
