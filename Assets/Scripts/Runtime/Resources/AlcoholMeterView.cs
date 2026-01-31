using RIEVES.GGJ2026.Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace RIEVES.GGJ2026.Runtime.Resources
{
    internal sealed class AlcoholMeterView : View
    {
        [SerializeField]
        private Image fillImage;

        public float Fill
        {
            set => fillImage.fillAmount = value;
        }
    }
}
