using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace RIEVES.GGJ2026.Core.Animations
{
    public sealed class CanvasGroupAnimation : TweenAnimation
    {
        [Header("General")]
        [SerializeField]
        private CanvasGroup target;

        [Header("Scale: Start Alpha")]
        [SerializeField]
        private bool isTweenFromStartAlpha = true;

        [Range(0f, 1f)]
        [SerializeField]
        private float startAlpha;

        [Header("Scale: End Alpha")]
        [SerializeField]
        private bool isTweenToEndAlpha;

        [Range(0f, 1f)]
        [SerializeField]
        private float endAlpha = 1f;

        [Header("Tween")]
        [SerializeField]
        private Ease ease = Ease.InOutCubic;

        [Range(0f, 60f)]
        [SerializeField]
        private float duration = 1f;

        [Range(0f, 60f)]
        [SerializeField]
        private float randomDurationOffset;

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
        private float initialAlpha;
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

        private void Awake()
        {
            if (target == false)
            {
                target = GetComponent<CanvasGroup>();
            }

            animationId = Guid.NewGuid().ToString();
        }

        private void Start()
        {
            initialAlpha = target.alpha;

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

            target.alpha = isTweenFromStartAlpha ? startAlpha : target.alpha;

            activeTween = target
                .DOFade(isTweenToEndAlpha ? endAlpha : initialAlpha, duration + Random.Range(0f, randomDurationOffset))
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
