using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RIEVES.GGJ2026.Core.Menus
{
    public sealed class MenuButtonElement :
        MonoBehaviour,
        ISelectHandler,
        IDeselectHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler,
        ISubmitHandler
    {
        [Header("General")]
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Image outlineImage;

        [Range(0f, 1f)]
        [SerializeField]
        private float disabledAlpha = 0.5f;

        [Header("Text")]
        [SerializeField]
        private TMP_Text titleText;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onClicked;

        [SerializeField]
        private UnityEvent onSelected;

        [SerializeField]
        private UnityEvent onDeselected;

        private bool IsInteractable
        {
            get => canvasGroup.interactable;
            set
            {
                canvasGroup.interactable = value;

                if (value)
                {
                    canvasGroup.alpha = 1f;
                }
                else
                {
                    canvasGroup.alpha = disabledAlpha;
                }
            }
        }

        public string Text
        {
            set => titleText.text = value;
        }

        public event Action OnClicked;

        public void OnSelect(BaseEventData eventData)
        {
            SetSelected(isSelected: true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetSelected(isSelected: false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(null);
            SetSelected(isSelected: true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetSelected(isSelected: false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsInteractable == false)
            {
                return;
            }

            OnClicked?.Invoke();
            onClicked.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (IsInteractable == false)
            {
                return;
            }

            OnClicked?.Invoke();
            onClicked.Invoke();
        }

        public void SetInteractable(bool isInteractable)
        {
            IsInteractable = isInteractable;
        }

        public void SetSelected(bool isSelected, bool isTriggerEvents = true)
        {
            outlineImage.enabled = isSelected;

            if (isTriggerEvents == false)
            {
                return;
            }

            if (isSelected)
            {
                onSelected.Invoke();
            }
            else
            {
                onDeselected.Invoke();
            }
        }
    }
}
