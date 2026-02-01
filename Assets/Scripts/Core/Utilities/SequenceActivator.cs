using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Utilities
{
    internal sealed class SequenceActivator : MonoBehaviour
    {
        [Serializable]
        private class SequenceEntry
        {
            [Min(0f)]
            [SerializeField]
            private float duration = 0.5f;

            [SerializeField]
            private UnityEvent onEnter;

            [SerializeField]
            private UnityEvent onExit;

            public float Duration => duration;

            public void TriggerEnter() => onEnter?.Invoke();
            public void TriggerExit() => onExit?.Invoke();
        }

        [SerializeField]
        private List<SequenceEntry> entries = new();

        private int currentIndex = -1;
        private float timer;
        private bool isRunning;

        private void Start()
        {
            foreach (var entry in entries)
            {
                entry.TriggerExit();
            }

            StartSequence();
        }

        public void StartSequence()
        {
            timer = 0f;
            currentIndex = -1;
            isRunning = true;
        }

        public void StopSequence()
        {
            if (!isRunning)
                return;

            if (currentIndex >= 0 && currentIndex < entries.Count)
            {
                entries[currentIndex].TriggerExit();
            }

            isRunning = false;
            currentIndex = -1;
            timer = 0f;
        }

        private void Update()
        {
            if (!isRunning || entries.Count == 0)
            {
                return;
            }

            if (currentIndex == -1)
            {
                Advance();
                return;
            }

            timer += Time.deltaTime;

            if (timer >= entries[currentIndex].Duration)
            {
                entries[currentIndex].TriggerExit();
                Advance();
            }
        }

        private void Advance()
        {
            timer = 0f;

            currentIndex++;

            if (currentIndex >= entries.Count)
            {
                currentIndex = 0;
            }

            entries[currentIndex].TriggerEnter();
        }
    }
}
