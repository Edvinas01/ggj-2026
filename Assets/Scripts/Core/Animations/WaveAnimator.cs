using UnityEngine;

namespace RIEVES.GGJ2026.Core.Animations
{
    public sealed class WaveAnimator : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private Transform target;

        [Header("Features")]
        [Range(0f, 100f)]
        [SerializeField]
        private float amplitude = 0.1f;

        [Range(0f, 100f)]
        [SerializeField]
        private float frequency = 2f;

        private Vector3 initialScale;
        private bool isPlaying;
        private float time;

        private void Awake()
        {
            initialScale = target.localScale;
        }

        private void Update()
        {
            if (isPlaying == false)
            {
                return;
            }

            time += Time.deltaTime;

            var scaleOffset = Mathf.Sin(time * frequency) * amplitude;
            target.localScale = initialScale * (1f + scaleOffset);
        }

        public void Play()
        {
            time = 0f;
            initialScale = target.localScale;
            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
            target.localScale = initialScale;
        }
    }
}
