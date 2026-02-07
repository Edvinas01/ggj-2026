using System.Linq;
using System.Threading;
using CHARK.GameManagement;
using Cysharp.Threading.Tasks;
using RIEVES.GGJ2026.Core.Views;
using RIEVES.GGJ2026.Runtime.Characters;
using RIEVES.GGJ2026.Runtime.Heat;
using UnityEngine;
using UnityEngine.Events;

namespace RIEVES.GGJ2026.Runtime.Stats
{
    internal sealed class StatsViewController : ViewController<StatsView>
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent onStatsShown;

        private StatsSystem statsSystem;
        private HeatSystem heatSystem;

        private CancellationTokenSource statsCancellationToken;

        protected override void Awake()
        {
            base.Awake();

            statsSystem = GameManager.GetSystem<StatsSystem>();
            heatSystem = GameManager.GetSystem<HeatSystem>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            View.OnShowEntered += OnViewShowEntered;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            View.OnShowEntered -= OnViewShowEntered;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            statsCancellationToken?.Cancel();
            statsCancellationToken?.Dispose();
            statsCancellationToken = null;
        }

        private void OnViewShowEntered()
        {
            statsCancellationToken?.Cancel();
            statsCancellationToken?.Dispose();
            statsCancellationToken = new CancellationTokenSource();
            ShowStatsAsync(statsCancellationToken.Token).Forget();
        }

        private async UniTaskVoid ShowStatsAsync(CancellationToken cancellationToken)
        {
            var correctConvos = statsSystem.GetConversations(ConversationController.ConversationResult.Correct).ToList();
            await View.CreateCorrectConvoElementsAsync(
                conversations: correctConvos,
                cancellationToken: cancellationToken
            );

            var incorrectConvos = statsSystem.GetConversations(ConversationController.ConversationResult.Incorrect).ToList();
            await View.CreateIncorrectConvoElementsAsync(
                conversations: incorrectConvos,
                cancellationToken: cancellationToken
            );

            var neutralConvos = statsSystem.GetConversations(ConversationController.ConversationResult.Neutral).ToList();
            await View.CreateNeutralConvoElementsAsync(
                conversations: neutralConvos,
                cancellationToken: cancellationToken
            );

            var usedItems = statsSystem.GetUsedItems().ToList();
            await View.CreateUsedItemElementsAsync(
                items: usedItems,
                cancellationToken: cancellationToken
            );

            var heat = heatSystem.CurrentHeat;

            var addedAlcoFromConvos = correctConvos.Sum(convo => convo.AddsAlcohol);
            var removedAlcoFromConvos = incorrectConvos.Sum(convo => convo.RemovesAlcohol);
            var neutralConvoCount = neutralConvos.Count;
            var addedAlcoFromItems = usedItems.Sum(item => item.Value);

            var totalScore = (int)((Mathf.Max(addedAlcoFromConvos + addedAlcoFromItems - removedAlcoFromConvos, 0) + neutralConvoCount) * heat);

            await View.SetHeatScoreAsync(heat, cancellationToken);
            await View.SetTotalScoreAsync(totalScore, cancellationToken);

            onStatsShown.Invoke();
        }
    }
}
