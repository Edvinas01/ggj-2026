using RIEVES.GGJ2026.Core.Views;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Resources
{
    internal sealed class AlcoholMeterViewController : ViewController<AlcoholMeterView>
    {
        [SerializeField]
        private ResourceController resourceController;

        protected override void Start()
        {
            base.Start();

            View.Fill = resourceController.AlcoholRatio;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            resourceController.OnAlcoholChanged += OnAlcoholChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            resourceController.OnAlcoholChanged -= OnAlcoholChanged;
        }

        private void OnAlcoholChanged(AlcoholChangedArgs args)
        {
            View.Fill = args.Ratio;
        }
    }
}
