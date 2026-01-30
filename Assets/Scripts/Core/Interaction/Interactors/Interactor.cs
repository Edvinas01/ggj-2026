using System;
using System.Collections.Generic;
using CHARK.GameManagement;
using RIEVES.GGJ2026.Core.Interaction.Interactables;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Core.Interaction.Interactors
{
    internal abstract class Interactor : MonoBehaviour, IInteractor
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events")]
        [Sirenix.OdinInspector.PropertyOrder(float.MaxValue)]
#else
        [Header("Events")]
#endif
        [SerializeField]
        public UnityEvent<InteractorHoverEnteredArgs> onHoverEntered;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events")]
        [Sirenix.OdinInspector.PropertyOrder(float.MaxValue)]
#endif
        [SerializeField]
        public UnityEvent<InteractorHoverExitedArgs> onHoverExited;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events")]
        [Sirenix.OdinInspector.PropertyOrder(float.MaxValue)]
#endif
        [SerializeField]
        public UnityEvent<InteractorSelectEnteredArgs> onSelectEntered;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events")]
        [Sirenix.OdinInspector.PropertyOrder(float.MaxValue)]
#endif
        [SerializeField]
        public UnityEvent<InteractorSelectExitedArgs> onSelectExited;

        private IInteractionSystem interactionSystem;

        private readonly List<IInteractable> hoveredInteractables = new();
        private readonly List<IInteractable> selectedInteractables = new();

        public string Name => name;

        public bool IsHovering => hoveredInteractables.Count > 0;

        public bool IsSelecting => selectedInteractables.Count > 0;

        protected IEnumerable<IInteractable> HoveredInteractables => hoveredInteractables;

        protected IEnumerable<IInteractable> SelectedInteractables => selectedInteractables;

        public event Action<InteractorHoverEnteredArgs> OnHoverEntered;

        public event Action<InteractorHoverExitedArgs> OnHoverExited;

        public event Action<InteractorSelectEnteredArgs> OnSelectEntered;

        public event Action<InteractorSelectExitedArgs> OnSelectExited;

        private void Awake()
        {
            interactionSystem = GameManager.GetSystem<IInteractionSystem>();
        }

        private void OnEnable()
        {
            interactionSystem.AddInteractor(this);
        }

        private void OnDisable()
        {
            UnHover();
            Deselect();

            interactionSystem.RemoveInteractor(this);
        }

        protected void FixedUpdate()
        {
            OnPhysicsUpdated();
        }

        protected void Update()
        {
            OnUpdated();
        }

        protected void LateUpdate()
        {
            OnLateUpdated();
        }

        protected virtual void OnPhysicsUpdated()
        {
        }

        protected virtual void OnUpdated()
        {
        }

        protected virtual void OnLateUpdated()
        {
        }

        public void Select()
        {
            if (hoveredInteractables.Count == 0)
            {
                return;
            }

            foreach (var hoveredInteractable in hoveredInteractables)
            {
                if (IsSelected(hoveredInteractable))
                {
                    continue;
                }

                selectedInteractables.Add(hoveredInteractable);
                hoveredInteractable.UnHover(this);

                var exitedArgs = new InteractorHoverExitedArgs(hoveredInteractable, this);
                OnHoverExited?.Invoke(exitedArgs);
                onHoverExited?.Invoke(exitedArgs);

                hoveredInteractable.Select(this);

                var enteredArgs = new InteractorSelectEnteredArgs(hoveredInteractable);
                OnSelectEntered?.Invoke(enteredArgs);
                onSelectEntered?.Invoke(enteredArgs);
            }

            hoveredInteractables.Clear();
        }

        public void Deselect(IInteractable interactable)
        {
            if (IsSelected(interactable) == false)
            {
                return;
            }

            selectedInteractables.Remove(interactable);

            interactable.Deselect(this);

            var args = new InteractorSelectExitedArgs(interactable);
            OnSelectExited?.Invoke(args);
            onSelectExited?.Invoke(args);
        }

        public void Deselect()
        {
            if (selectedInteractables.Count == 0)
            {
                return;
            }

            foreach (var selectedInteractable in selectedInteractables)
            {
                selectedInteractable.Deselect(this);

                var args = new InteractorSelectExitedArgs(selectedInteractable);
                OnSelectExited?.Invoke(args);
                onSelectExited?.Invoke(args);
            }

            selectedInteractables.Clear();
        }

        /// <summary>
        /// Add given <paramref name="interactable"/> to <see cref="hoveredInteractables"/> list.
        /// </summary>
        protected void Hover(IInteractable interactable)
        {
            if (IsHovered(interactable) || IsSelected(interactable))
            {
                return;
            }

            hoveredInteractables.Add(interactable);
            interactable.Hover(this);

            var args = new InteractorHoverEnteredArgs(interactable, this);
            OnHoverEntered?.Invoke(args);
            onHoverEntered?.Invoke(args);
        }

        /// <summary>
        /// Clear <see cref="hoveredInteractables"/> list and call
        /// <see cref="UnHover"/> on each.
        /// </summary>
        protected void UnHover()
        {
            if (hoveredInteractables.Count == 0)
            {
                return;
            }

            foreach (var hoveredInteractable in hoveredInteractables)
            {
                hoveredInteractable.UnHover(this);

                var args = new InteractorHoverExitedArgs(hoveredInteractable, this);
                OnHoverExited?.Invoke(args);
                onHoverExited?.Invoke(args);
            }

            hoveredInteractables.Clear();
        }

        /// <returns>
        /// <c>true</c> if <paramref name="interactable"/> is being hovered by this interactor or
        /// <c>false</c> otherwise.
        /// </returns>
        protected bool IsHovered(IInteractable interactable)
        {
            return hoveredInteractables.Contains(interactable);
        }

        /// <returns>
        /// <c>true</c> if <paramref name="interactable"/> is being selected by this interactor or
        /// <c>false</c> otherwise.
        /// </returns>
        protected bool IsSelected(IInteractable interactable)
        {
            return selectedInteractables.Contains(interactable);
        }
    }
}
