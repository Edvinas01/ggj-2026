using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RIEVES.GGJ2026.Core.Views
{
    public abstract class ViewController<TView> : ViewController where TView : View
    {
        [Serializable]
        private sealed class ControllerData
        {
            [SerializeField]
            private TView view;

            [SerializeField]
            private Transform viewParent;

            [SerializeField]
            private StartMode startMode = StartMode.Show;

            public TView View => view;

            public Transform ViewParent => viewParent;

            public StartMode StartMode => startMode;
        }

        private enum StartMode
        {
            None = 0,
            Show = 1,
            Hide = 2,
        }

        [SerializeField]
        private ControllerData controllerData;

        private TView currentView;

        protected TView View
        {
            get
            {
                if (currentView == false)
                {
                    currentView = CreateView();
                }

                return currentView;
            }
        }

        protected virtual void Awake()
        {
            var view = controllerData.View;
            if (view == false)
            {
                Debug.LogError($"{nameof(controllerData.View)} must be set to a prefab or an instance", this);
                enabled = false;
                return;
            }

            currentView = view.gameObject.scene.IsValid() ? view : CreateView();
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void Start()
        {
            switch (controllerData.StartMode)
            {
                case StartMode.Show:
                {
                    View.EnableCanvas();
                    View.EnableInteraction();
                    break;
                }
                case StartMode.Hide:
                {
                    View.DisableCanvas();
                    View.DisableInteraction();
                    break;
                }
                case StartMode.None:
                default:
                {
                    break;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            currentView = null;
        }

        protected virtual void Update()
        {
        }

        public override void InitializeView()
        {
            if (currentView)
            {
                return;
            }

#if UNITY_EDITOR
            var undoIndex = UnityEditor.Undo.GetCurrentGroup();
            UnityEditor.Undo.SetCurrentGroupName(nameof(InitializeView));
            UnityEditor.Undo.RecordObject(this, nameof(InitializeView));
#endif
            currentView = CreateView();

#if UNITY_EDITOR
            UnityEditor.Undo.CollapseUndoOperations(undoIndex);
#endif
        }

        public override void ShowView()
        {
            if (View.State is ViewState.Shown or ViewState.Showing)
            {
                return;
            }

            View.Show();
        }

        public override void HideView()
        {
            if (View.State is ViewState.Hidden or ViewState.Hiding)
            {
                return;
            }

            View.Hide();
        }

        public override async UniTask ShowViewAsync(CancellationToken cancellationToken = default)
        {
            if (View.State is ViewState.Shown or ViewState.Showing)
            {
                return;
            }

            await View.ShowAsync(cancellationToken);
        }

        public override async UniTask HideViewAsync(CancellationToken cancellationToken = default)
        {
            if (View.State is ViewState.Hidden or ViewState.Hiding)
            {
                return;
            }

            await View.HideAsync(cancellationToken);
        }

        private TView CreateView()
        {
            var parent = controllerData.ViewParent ? controllerData.ViewParent : transform;
            var view = controllerData.View;

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                var newView = (TView)UnityEditor.PrefabUtility.InstantiatePrefab(view, parent);
                UnityEditor.Undo.RegisterCreatedObjectUndo(
                    newView.gameObject,
                    $"Create view {newView.name}"
                );

                newView.name = view.name;
                return newView;
            }
#endif

            var instance = Instantiate(view, parent);
            instance.name = view.name;

            return instance;
        }
    }

    public abstract class ViewController : MonoBehaviour
    {
        public abstract void InitializeView();

        public abstract void ShowView();

        public abstract void HideView();

        public abstract UniTask ShowViewAsync(CancellationToken cancellationToken = default);

        public abstract UniTask HideViewAsync(CancellationToken cancellationToken = default);
    }
}
