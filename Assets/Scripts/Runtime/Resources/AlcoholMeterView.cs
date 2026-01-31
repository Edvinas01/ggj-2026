using RIEVES.GGJ2026.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RIEVES.GGJ2026.Runtime.Resources
{
    internal sealed class AlcoholMeterView : View
    {
        [SerializeField]
        private Image fillImage;

        [SerializeField]
        private TMP_Text fillText;

        public float Fill
        {
            set
            {
                fillImage.fillAmount = value;
                fillText.text = value.ToString("P0");
            }
        }
    }
}
