using CHARK.GameManagement;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Pausing
{
    internal sealed class PauseTrigger : MonoBehaviour
    {
        [Header("Features")]
        [SerializeField]
        private bool isTriggerOnStart;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onPaused;

        [SerializeField]
        private UnityEvent onResumed;

        private IPauseSystem pauseSystem;

        private void Awake()
        {
            pauseSystem = GameManager.GetSystem<IPauseSystem>();
        }

        private void Start()
        {
            if (isTriggerOnStart == false)
            {
                return;
            }

            if (pauseSystem.IsPaused)
            {
                onPaused.Invoke();
            }
            else
            {
                onResumed.Invoke();
            }
        }

        private void OnEnable()
        {
            GameManager.AddListener<GamePausedMessage>(OnGamePaused);
            GameManager.AddListener<GameResumedMessage>(OnGameResumed);
        }

        private void OnDisable()
        {
            GameManager.RemoveListener<GamePausedMessage>(OnGamePaused);
            GameManager.RemoveListener<GameResumedMessage>(OnGameResumed);
        }

        private void OnGamePaused(GamePausedMessage message)
        {
            onPaused.Invoke();
        }

        private void OnGameResumed(GameResumedMessage message)
        {
            onResumed.Invoke();
        }
    }
}
