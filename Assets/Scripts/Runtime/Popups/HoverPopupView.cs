using RIEVES.GGJ2026.Core.Views;
using TMPro;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Popups
{
    internal sealed class HoverPopupView : View
    {
        [Header("Text")]
        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private RectTransform interactionInfo;

        public string TitleText
        {
            set => titleText.text = value;
        }

        public bool IsBlocked
        {
            set => interactionInfo.gameObject.SetActive(value == false);
        }
    }
}
