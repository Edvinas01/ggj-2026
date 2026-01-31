using System;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class CharacterAnimationController : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField]
        private Animator targetAnim;

        [SerializeField]
        private string stateName;

        [Header("Rendering")]
        [SerializeField]
        private bool isPlayOnStart;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onPlay;

        [SerializeField]
        private UnityEvent onStop;

        public bool IsPlaying
        {
            get
            {
                var stateInfo = targetAnim.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(stateName) && stateInfo.normalizedTime < 1f)
                {
                    return true;
                }

                return false;
            }
        }

        private bool isPlayRequested;

        public event Action OnPlay;
        public event Action OnStop;

        private void Start()
        {
            if (isPlayOnStart)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }

        private void Update()
        {
            var isPlayPrev = isPlayRequested;
            var isPlayNext = IsPlaying;

            if (isPlayPrev == isPlayNext)
            {
                return;
            }

            if (isPlayNext)
            {
                OnPlay?.Invoke();
                onPlay.Invoke();
            }
            else
            {
                OnStop?.Invoke();
                onStop.Invoke();
            }

            isPlayRequested = isPlayNext;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            isPlayRequested = true;
            targetAnim.enabled = true;
            targetAnim.Play(stateName, -1, 0f);
            OnPlay?.Invoke();
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            targetAnim.Play(targetAnim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            targetAnim.Update(0f);
            targetAnim.enabled = false;
            OnStop?.Invoke();
        }
    }
}
