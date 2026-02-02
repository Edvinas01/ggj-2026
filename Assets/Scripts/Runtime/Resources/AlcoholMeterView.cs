using System.Collections.Generic;
using RIEVES.GGJ2026.Core.Animations;
using RIEVES.GGJ2026.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RIEVES.GGJ2026.Runtime.Resources
{
    internal sealed class AlcoholMeterView : View
    {
        private sealed class FloatingText
        {
            public TMP_Text Text { get; set; }

            public TweenAnimation HideAnim { get; set; }

            public float Lifetime { get; set; }

            public FloatingText(TMP_Text text, TweenAnimation hideAnim, float lifetime)
            {
                Text = text;
                HideAnim = hideAnim;
                Lifetime = lifetime;
            }
        }

        [Header("Fill")]
        [SerializeField]
        private Image fillImage;

        [SerializeField]
        private TMP_Text fillText;

        [Header("Floaters")]
        [SerializeField]
        private GameObject floatTextTemplate;

        [SerializeField]
        private Color positiveColor = Color.green;

        [SerializeField]
        private Color negativeColor = Color.red;

        [Min(0f)]
        [SerializeField]
        private float floatSpeed = 1f;

        [Min(0f)]
        [SerializeField]
        private float floatLifetime = 2f;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onFillChanged;

        private readonly List<FloatingText> texts = new();

        public float Fill
        {
            set
            {
                fillImage.fillAmount = value;
                fillText.text = value.ToString("P0");
                onFillChanged.Invoke();
            }
        }

        private void Update()
        {
            for (var index = texts.Count - 1; index >= 0; index--)
            {
                var floatyText = texts[index];
                var deltaTime = Time.deltaTime;
                floatyText.Lifetime -= deltaTime;

                var textTransform = (RectTransform)floatyText.Text.transform;
                var position = textTransform.position;
                position.y += floatSpeed * deltaTime;

                textTransform.position = position;

                if (floatyText.Lifetime < 0 && floatyText.HideAnim.IsPlaying == false)
                {
                    floatyText.HideAnim.Play();
                    floatyText.HideAnim.OnPlayExited += () =>
                    {
                        Destroy(floatyText.Text.gameObject);
                        texts.Remove(floatyText);
                    };
                }
            }
        }

        public void SpawnIndicator(int value)
        {
            var instance = Instantiate(floatTextTemplate, floatTextTemplate.transform.parent);
            var tmpText = instance.GetComponent<TMP_Text>();
            var hideAnim = instance.GetComponent<TweenAnimation>();

            if (value > 0)
            {
                tmpText.color = positiveColor;
                tmpText.text = $"+{value}";
            }
            else if (value < 0)
            {
                tmpText.color = negativeColor;
                tmpText.text = $"{value}";
            }
            else
            {
                tmpText.color = Color.white;
                tmpText.text = $"{value}";
            }

            instance.gameObject.SetActive(true);

            texts.Add(new FloatingText(tmpText, hideAnim, floatLifetime));
        }
    }
}
