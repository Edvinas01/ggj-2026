using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace RIEVES.GGJ2026.Core.Animations
{
    public sealed class ScaleAnimation : TweenAnimation
    {
        [Header("General")]
        [SerializeField]
        private Transform target;

        [Header("Scale: Start Scale")]
        [SerializeField]
        private bool isTweenFromStartScale = true;

        [SerializeField]
        private Vector3 startScale = Vector3.zero;

        [Min(0f)]
        [SerializeField]
        private float startScaleMultiplier = 1f;

        [Header("Scale: End Scale")]
        [SerializeField]
        private bool isTweenToEndScale;

        [SerializeField]
        private Vector3 endScale = Vector3.one;

        [Header("Tween")]
        [SerializeField]
        private Ease ease = Ease.InOutCubic;

        [Range(0f, 60f)]
        [SerializeField]
        private float duration = 1f;

        [Range(0f, 60f)]
        [SerializeField]
        private float randomDurationOffset = 0.5f;

        [Header("Features")]
        [SerializeField]
        private bool isPlayOnStart;

        [SerializeField]
        private bool isDestroyOnCompleted;

        [SerializeField]
        private bool isIndependentUpdate = true;

        [SerializeField]
        private bool isPlayOnce;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onCompleted;

        private Tween activeTween;
        private bool isTriggeredOnce;
        private Vector3 initialScale;
        private string animationId;

        public override bool IsPlaying
        {
            get
            {
                if (activeTween == null)
                {
                    return false;
                }

                if (activeTween.IsActive() == false)
                {
                    return false;
                }

                return activeTween.IsPlaying();
            }
        }

        public override event Action OnPlayEntered;
        public override event Action OnPlayExited;

        private void Reset()
        {
            target = transform;
        }

        private void Awake()
        {
            if (target == false)
            {
                target = transform;
            }

            animationId = Guid.NewGuid().ToString();
        }

        private void Start()
        {
            initialScale = target.localScale;

            if (isPlayOnStart)
            {
                Play();
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(animationId);
        }

        public override void Play()
        {
            if (isPlayOnce && isTriggeredOnce)
            {
                return;
            }

            isTriggeredOnce = true;
            DOTween.Kill(animationId);

            PlayTween();
        }

        public override async UniTask PlayAsync(CancellationToken cancellationToken = default)
        {
            if (isPlayOnce && isTriggeredOnce)
            {
                return;
            }

            isTriggeredOnce = true;
            DOTween.Kill(animationId);

            await PlayTween().WithCancellation(cancellationToken: cancellationToken);
        }

        private Tween PlayTween()
        {
            OnTweenEntered();

            target.localScale = isTweenFromStartScale ? startScale * startScaleMultiplier : target.localScale * startScaleMultiplier;

            activeTween = target
                .DOScale(isTweenToEndScale ? endScale : initialScale, duration + Random.Range(0f, randomDurationOffset))
                .SetEase(ease)
                .SetId(animationId)
                .SetUpdate(isIndependentUpdate: isIndependentUpdate)
                .OnComplete(OnTweenExited);

            return activeTween;
        }

        public override void Stop()
        {
            DOTween.Kill(animationId);
        }

        private void OnTweenEntered()
        {
            OnPlayEntered?.Invoke();
        }

        private void OnTweenExited()
        {
            OnPlayExited?.Invoke();
            onCompleted.Invoke();

            if (isDestroyOnCompleted)
            {
                Destroy(this);
            }
        }
    }
}
