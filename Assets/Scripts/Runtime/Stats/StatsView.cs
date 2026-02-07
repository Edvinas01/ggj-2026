using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using RIEVES.GGJ2026.Core.Animations;
using RIEVES.GGJ2026.Core.Views;
using RIEVES.GGJ2026.Runtime.Characters;
using RIEVES.GGJ2026.Runtime.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RIEVES.GGJ2026.Runtime.Stats
{
    internal sealed class StatsView : View
    {
        [Header("Convo: Correct")]
        [SerializeField]
        private RectTransform correctConvoParent;

        [SerializeField]
        private TMP_Text correctConvoTitle;

        [SerializeField]
        private Image correctConvoIconTemplate;

        [Header("Convo: Incorrect")]
        [SerializeField]
        private RectTransform incorrectConvoParent;

        [SerializeField]
        private TMP_Text incorrectConvoTitle;

        [SerializeField]
        private Image incorrectConvoIconTemplate;

        [Header("Convo: Neutral")]
        [SerializeField]
        private RectTransform neutralConvoParent;

        [SerializeField]
        private TMP_Text neutralConvoTitle;

        [SerializeField]
        private Image neutralConvoIconTemplate;

        [Header("Items Used")]
        [SerializeField]
        private RectTransform usedItemParent;

        [SerializeField]
        private TMP_Text usedItemTitle;

        [SerializeField]
        private Image itemUsedIconTemplate;

        [Header("Total Score")]
        [SerializeField]
        private TMP_Text heatScoreText;

        [SerializeField]
        private TMP_Text totalScoreText;

        private readonly List<Image> iconElements = new();

        protected override void Start()
        {
            base.Start();

            correctConvoParent.gameObject.SetActive(false);
            incorrectConvoParent.gameObject.SetActive(false);
            neutralConvoParent.gameObject.SetActive(false);
            usedItemParent.gameObject.SetActive(false);

            heatScoreText.gameObject.SetActive(false);
            totalScoreText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            foreach (var iconElement in iconElements)
            {
                Destroy(iconElement.sprite);
            }

            iconElements.Clear();
        }

        public async UniTask CreateCorrectConvoElementsAsync(IEnumerable<CharacterData> conversations, CancellationToken cancellationToken)
        {
            var convoList = conversations.ToList();
            if (convoList.Count <= 0)
            {
                correctConvoParent.gameObject.SetActive(false);
                return;
            }

            correctConvoParent.gameObject.SetActive(true);

            var count = 1;
            foreach (var conversation in convoList)
            {
                await UniTask.WhenAll(
                    SetTextAndAnimateAsync(correctConvoTitle, $"Gavai pagert: {count}", cancellationToken),
                    CreateIconElementAsync(correctConvoParent, correctConvoIconTemplate, conversation.FrontTexture, cancellationToken)
                );

                count++;
            }
        }

        public async UniTask CreateIncorrectConvoElementsAsync(IEnumerable<CharacterData> conversations, CancellationToken cancellationToken)
        {
            var convoList = conversations.ToList();
            if (convoList.Count <= 0)
            {
                incorrectConvoParent.gameObject.SetActive(false);
                return;
            }

            incorrectConvoParent.gameObject.SetActive(true);

            var count = 1;
            foreach (var conversation in convoList)
            {
                await UniTask.WhenAll(
                    SetTextAndAnimateAsync(incorrectConvoTitle, $"Prasileidai: {count}", cancellationToken),
                    CreateIconElementAsync(incorrectConvoParent, incorrectConvoIconTemplate, conversation.FrontTexture, cancellationToken)
                );

                count++;
            }
        }

        public async UniTask CreateNeutralConvoElementsAsync(IEnumerable<CharacterData> conversations, CancellationToken cancellationToken)
        {
            var convoList = conversations.ToList();
            if (convoList.Count <= 0)
            {
                neutralConvoParent.gameObject.SetActive(false);
                return;
            }

            neutralConvoParent.gameObject.SetActive(true);

            var count = 1;
            foreach (var conversation in convoList)
            {
                await UniTask.WhenAll(
                    SetTextAndAnimateAsync(neutralConvoTitle, $"Šeip pakalbėjai: {count}", cancellationToken),
                    CreateIconElementAsync(neutralConvoParent, neutralConvoIconTemplate, conversation.FrontTexture, cancellationToken)
                );

                count++;
            }
        }

        public async UniTask CreateUsedItemElementsAsync(IEnumerable<ItemData> items, CancellationToken cancellationToken)
        {
            var itemList = items.ToList();
            if (itemList.Count <= 0)
            {
                usedItemParent.gameObject.SetActive(false);
                return;
            }

            usedItemParent.gameObject.SetActive(true);

            var count = 1;
            foreach (var itemData in itemList)
            {
                await UniTask.WhenAll(
                    SetTextAndAnimateAsync(usedItemTitle, $"Suvartojai: {count}", cancellationToken),
                    CreateIconElementAsync(usedItemParent, itemUsedIconTemplate, itemData.Texture, cancellationToken)
                );

                count++;
            }
        }

        public async UniTask SetHeatScoreAsync(float heatScore, CancellationToken cancellationToken)
        {
            await SetTextAndAnimateAsync(heatScoreText, $"Karštis: {heatScore:F2}", cancellationToken);
        }

        public async UniTask SetTotalScoreAsync(int totalScore, CancellationToken cancellationToken)
        {
            await SetTextAndAnimateAsync(totalScoreText, $"Rezultatas: {totalScore}", cancellationToken);
        }

        private static async UniTask SetTextAndAnimateAsync(TMP_Text tmpText, string value, CancellationToken cancellationToken)
        {
            tmpText.gameObject.SetActive(true);
            tmpText.text = value;

            if (tmpText.TryGetComponent<StudioEventEmitter>(out var emitter))
            {
                emitter.Play();
            }

            if (tmpText.TryGetComponent<TweenAnimation>(out var anim))
            {
                await anim.PlayAsync(cancellationToken);
            }
        }

        private async UniTask CreateIconElementAsync(RectTransform parent, Image template, Texture2D icon, CancellationToken token)
        {
            var instance = Instantiate(template, parent);

            var sprite = Sprite.Create(
                icon,
                new Rect(0, 0, icon.width, icon.height),
                new Vector2(0.5f, 0.5f)
            );

            instance.sprite = sprite;
            instance.gameObject.SetActive(true);
            if (instance.TryGetComponent<TweenAnimation>(out var anim))
            {
                await anim.PlayAsync(token);
            }

            iconElements.Add(instance);
        }
    }
}
