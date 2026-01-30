using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Animations
{
    public abstract class TweenAnimation : MonoBehaviour
    {
        public abstract event Action OnPlayEntered;

        public abstract event Action OnPlayExited;

        public abstract bool IsPlaying { get; }

        public abstract void Play();

        public abstract UniTask PlayAsync(CancellationToken cancellationToken = default);

        public abstract void Stop();
    }
}
