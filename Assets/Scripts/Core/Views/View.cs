using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RIEVES.GGJ2026.Core.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RIEVES.GGJ2026.Core.Views
{
    public abstract class View : MonoBehaviour
    {
        [Serializable]
        private sealed class ViewData
        {
            [Header("UI")]
            [SerializeField]
            private Canvas canvas;

            [SerializeField]
            private CanvasGroup canvasGroup;

            [Header("Selection")]
            [SerializeField]
            private GameObject gameObjectToSelect;

            [SerializeField]
            private bool isAutoSelectGameObject;

            [SerializeField]
            private bool isAutoDeselectGameObject;

            [SerializeField]
            private bool isSelectLastGameObject;

            [Header("Animation")]
            [SerializeField]
            private TweenAnimation showAnimation;

            [SerializeField]
            private TweenAnimation hideAnimation;

            [Header("Events")]
            [SerializeField]
            private UnityEvent onShowEntered;

            [SerializeField]
            private UnityEvent onShowExited;

            [SerializeField]
            private UnityEvent onHideEntered;

            [SerializeField]
            private UnityEvent onHideExited;

            public Canvas Canvas
            {
                get => canvas;
                set => canvas = value;
            }

            public CanvasGroup CanvasGroup
            {
                get => canvasGroup;
                set => canvasGroup = value;
            }

            public GameObject GameObjectToSelect
            {
                get => gameObjectToSelect;
                set => gameObjectToSelect = value;
            }

            public bool IsAutoSelectGameObject => isAutoSelectGameObject;

            public bool IsAutoDeselectGameObject => isAutoDeselectGameObject;

            public bool IsSelectLastGameObject => isSelectLastGameObject;

            public TweenAnimation ShowAnimation => showAnimation;

            public TweenAnimation HideAnimation => hideAnimation;

            public UnityEvent OnShowEntered => onShowEntered;

            public UnityEvent OnShowExited => onShowExited;

            public UnityEvent OnHideEntered => onHideEntered;

            public UnityEvent OnHideExited => onHideExited;
        }

        [SerializeField]
        private ViewData viewData;

        protected GameObject GameObjectToSelect
        {
            get => viewData.GameObjectToSelect;
            set => viewData.GameObjectToSelect = value;
        }

        protected GameObject LastSelectedGameObject { get; private set; }

        public ViewState State
        {
            get
            {
                if (viewData.ShowAnimation && viewData.ShowAnimation.IsPlaying)
                {
                    return ViewState.Showing;
                }

                if (viewData.HideAnimation && viewData.HideAnimation.IsPlaying)
                {
                    return ViewState.Hiding;
                }

                if (viewData.Canvas.enabled)
                {
                    return ViewState.Shown;
                }

                return ViewState.Hidden;
            }
        }

        public event Action OnShowEntered;

        public event Action OnShowExited;

        public event Action OnHideEntered;

        public event Action OnHideExited;

        protected virtual void Awake()
        {
            if (viewData.Canvas == false)
            {
                viewData.Canvas = GetComponent<Canvas>();
            }

            if (viewData.Canvas == false)
            {
                Debug.LogError($"{nameof(viewData.Canvas)} must be set", this);
                enabled = false;
                return;
            }

            if (viewData.CanvasGroup == false)
            {
                viewData.CanvasGroup = GetComponent<CanvasGroup>();
            }

            if (viewData.CanvasGroup == false)
            {
                Debug.LogError($"{nameof(viewData.CanvasGroup)} must be set", this);
                enabled = false;
            }
        }

        protected virtual void OnEnable()
        {
            if (viewData.ShowAnimation)
            {
                viewData.ShowAnimation.OnPlayEntered += OnViewShowEntered;
                viewData.ShowAnimation.OnPlayExited += OnViewShowExited;
            }

            if (viewData.HideAnimation)
            {
                viewData.HideAnimation.OnPlayEntered += OnViewHideEntered;
                viewData.HideAnimation.OnPlayExited += OnViewHideExited;
            }
        }

        protected virtual void OnDisable()
        {
            if (viewData.ShowAnimation)
            {
                viewData.ShowAnimation.OnPlayEntered -= OnViewShowEntered;
                viewData.ShowAnimation.OnPlayExited -= OnViewShowExited;
            }

            if (viewData.HideAnimation)
            {
                viewData.HideAnimation.OnPlayEntered -= OnViewHideEntered;
                viewData.HideAnimation.OnPlayExited -= OnViewHideExited;
            }
        }

        protected virtual void Start()
        {
        }

        public virtual void Show()
        {
            Show(isAnimate: true);
        }

        public virtual void Hide()
        {
            Hide(isAnimate: true);
        }

        public virtual void Show(bool isAnimate)
        {
            if (isAnimate && viewData.ShowAnimation)
            {
                if (viewData.HideAnimation)
                {
                    viewData.HideAnimation.Stop();
                }

                viewData.ShowAnimation.Play();
            }
            else
            {
                OnViewShowEntered();
                OnViewShowExited();
            }
        }

        public virtual void Hide(bool isAnimate)
        {
            if (isAnimate && viewData.HideAnimation)
            {
                if (viewData.ShowAnimation)
                {
                    viewData.ShowAnimation.Stop();
                }

                viewData.HideAnimation.Play();
            }
            else
            {
                OnViewHideEntered();
                OnViewHideExited();
            }
        }

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            if (viewData.ShowAnimation)
            {
                if (viewData.HideAnimation)
                {
                    viewData.HideAnimation.Stop();
                }

                await viewData.ShowAnimation.PlayAsync(cancellationToken);
            }
            else
            {
                OnViewShowEntered();
                OnViewShowExited();
            }
        }

        public virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            if (viewData.HideAnimation)
            {
                if (viewData.ShowAnimation)
                {
                    viewData.ShowAnimation.Stop();
                }

                await viewData.HideAnimation.PlayAsync(cancellationToken);
            }
            else
            {
                OnViewHideEntered();
                OnViewHideExited();
            }
        }

        public virtual void EnableCanvas()
        {
            viewData.Canvas.enabled = true;
        }

        public virtual void DisableCanvas()
        {
            viewData.Canvas.enabled = false;
        }

        public virtual void EnableInteraction()
        {
            viewData.CanvasGroup.interactable = true;
        }

        public virtual void DisableInteraction()
        {
            viewData.CanvasGroup.interactable = false;
        }

        public virtual void SelectGameObject()
        {
            if (viewData.IsSelectLastGameObject && LastSelectedGameObject)
            {
                EventSystem.current.SetSelectedGameObject(LastSelectedGameObject);
            }
            else if (viewData.GameObjectToSelect)
            {
                EventSystem.current.SetSelectedGameObject(viewData.GameObjectToSelect);
            }
        }

        public virtual void DeselectGameObject()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        protected virtual void OnViewShowEntered()
        {
            EnableCanvas();
            EnableInteraction();

            OnShowEntered?.Invoke();
            viewData.OnShowEntered.Invoke();
        }

        protected virtual void OnViewShowExited()
        {
            if (viewData.IsAutoSelectGameObject)
            {
                SelectGameObject();
            }

            OnShowExited?.Invoke();
            viewData.OnShowExited.Invoke();
        }

        protected virtual void OnViewHideEntered()
        {
            LastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            DisableInteraction();

            if (viewData.IsAutoDeselectGameObject)
            {
                DeselectGameObject();
            }

            OnHideEntered?.Invoke();
            viewData.OnHideEntered.Invoke();
        }

        protected virtual void OnViewHideExited()
        {
            DisableCanvas();

            OnHideExited?.Invoke();
            viewData.OnHideExited.Invoke();
        }
    }
}
