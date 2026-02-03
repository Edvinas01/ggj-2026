using System;
using System.Collections;
using RIEVES.GGJ2026.Core.Menus;
using RIEVES.GGJ2026.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RIEVES.GGJ2026.Runtime.Beginning
{
    internal sealed class BeginGameView : View
    {
        [SerializeField]
        private TMP_Text loadingText;

        [SerializeField]
        private MenuButtonElement beginButton;

        [SerializeField]
        private UnityEvent onBeginButtonEnabled;


        public bool IsBeginButtonEnabled
        {
            set
            {
                beginButton.gameObject.SetActive(value);

                if (value)
                {
                    StartCoroutine(SelectButtonRoutine());
                    onBeginButtonEnabled.Invoke();
                }
            }
        }

        public bool IsLoadingTextEnabled
        {
            set => loadingText.enabled = value;
        }

        public event Action OnBeginClicked;

        protected override void OnEnable()
        {
            base.OnEnable();

            beginButton.OnClicked += OnBeginButtonClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            beginButton.OnClicked -= OnBeginButtonClicked;
        }

        private void OnBeginButtonClicked()
        {
            OnBeginClicked?.Invoke();
        }

        private IEnumerator SelectButtonRoutine()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(beginButton.gameObject);
        }
    }
}
