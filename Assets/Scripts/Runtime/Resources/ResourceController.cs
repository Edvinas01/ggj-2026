using System;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Resources
{
    internal sealed class ResourceController : MonoBehaviour
    {
        [Min(0)]
        [SerializeField]
        private int alcohol;

        [Min(0)]
        [SerializeField]
        private int alcoholMax = 100;

        public float AlcoholRatio => alcohol / (float)alcoholMax;

        public event Action<AlcoholChangedArgs> OnAlcoholChanged;

        public void AddAlcohol(int amount)
        {
            var alcoholPrev = alcohol;
            var alcoholNext = Math.Min(alcoholMax, alcohol + Math.Max(0, amount));

            if (alcoholPrev == alcoholNext)
            {
                return;
            }

            var prevRatio = AlcoholRatio;
            alcohol = alcoholNext;

            OnAlcoholChanged?.Invoke(new AlcoholChangedArgs(prevRatio, AlcoholRatio));
        }

        public void UseAlcohol(int amount)
        {
            var alcoholPrev = alcohol;
            var alcoholNext = Math.Max(0, alcohol - Math.Max(0, amount));

            if (alcoholPrev == alcoholNext)
            {
                return;
            }

            var prevRatio = AlcoholRatio;
            alcohol = alcoholNext;

            OnAlcoholChanged?.Invoke(new AlcoholChangedArgs(prevRatio, AlcoholRatio));
        }
    }
}
