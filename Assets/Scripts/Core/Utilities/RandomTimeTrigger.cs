using UnityEngine;
using UnityEngine.Events;

namespace InSun.JamOne.Core.Utilities
{
    internal sealed class RandomEventTrigger : MonoBehaviour
    {
        [SerializeField]
        private Vector2 triggerDuration = new(5f, 10f);

        [SerializeField]
        private UnityEvent onTrigger;

        private float nextTriggerTime;

        private void Start()
        {
            ScheduleNextTrigger();
        }

        private void Update()
        {
            if (Time.time < nextTriggerTime)
            {
                return;
            }

            ScheduleNextTrigger();
            onTrigger.Invoke();
        }

        private void ScheduleNextTrigger()
        {
            var offset = Random.Range(triggerDuration.x, triggerDuration.y);
            nextTriggerTime = Time.time + offset;
        }
    }
}
