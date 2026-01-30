using System;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Gameplay
{
    public abstract class GameState : ScriptableObject
    {
        protected enum Status
        {
            Completed,
            Working,
        }

        public string Name => GetType().Name;

        protected GameState NextState { get; set; }

        public void Initialize()
        {
            OnInitialized();
        }

        public void Dispose()
        {
            OnDisposed();
        }

        public void Enter()
        {
            OnEntered();
        }

        public void Exit()
        {
            OnExited();
        }

        public GameState Update()
        {
            var status = OnUpdated();
            return status switch
            {
                Status.Working => this,
                Status.Completed => NextState,
                _ => throw new ArgumentOutOfRangeException($"Unsuported status: {status}")
            };
        }

        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnDisposed()
        {
        }

        protected virtual void OnEntered()
        {
        }

        protected virtual void OnExited()
        {
        }

        protected abstract Status OnUpdated();
    }
}
