using RIEVES.GGJ2026.Core.Menus;
using RIEVES.GGJ2026.Core.Views;
using TMPro;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationView : View
    {
        [Header("Elements")]
        [SerializeField]
        private MenuButtonElement choiceButtonPrefab;

        [SerializeField]
        private RectTransform choiceParent;

        [Header("Text")]
        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private TMP_Text contentText;

        public string TitleText
        {
            set => titleText.text = value;
        }

        public string ContentText
        {
            set => contentText.text = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}
