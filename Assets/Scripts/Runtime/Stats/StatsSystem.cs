using System.Collections.Generic;
using System.Linq;
using CHARK.GameManagement;
using CHARK.GameManagement.Systems;
using RIEVES.GGJ2026.Core.Scenes;
using RIEVES.GGJ2026.Runtime.Characters;
using RIEVES.GGJ2026.Runtime.Items;

namespace RIEVES.GGJ2026.Runtime.Stats
{
    internal sealed class StatsSystem : MonoSystem
    {
        private ISceneSystem sceneSystem;

        private readonly List<ConversationStat> recordedConversations = new();
        private readonly List<ItemUsedStat> recordedUsedItems = new();

        public override void OnInitialized()
        {
            base.OnInitialized();

            sceneSystem = GameManager.GetSystem<ISceneSystem>();

            GameManager.AddListener<SceneLoadExitedMessage>(OnSceneLoadExited);
        }

        public override void OnDisposed()
        {
            base.OnDisposed();

            GameManager.RemoveListener<SceneLoadExitedMessage>(OnSceneLoadExited);
        }

        public IEnumerable<CharacterData> GetConversations(ConversationController.ConversationResult result)
        {
            return recordedConversations
                .Where(conversation => conversation.Result == result)
                .Select(conversation => conversation.CharacterData);
        }

        public IEnumerable<ItemData> GetUsedItems()
        {
            return recordedUsedItems.Select(item => item.ItemData);
        }

        public void RecordStat(ConversationStat stat)
        {
            recordedConversations.Add(stat);
        }

        public void RecordStat(ItemUsedStat stat)
        {
            recordedUsedItems.Add(stat);
        }

        private void OnSceneLoadExited(SceneLoadExitedMessage message)
        {
            if (sceneSystem.IsStartingScene(message.Collection))
            {
                ResetStats();
            }
        }

        private void ResetStats()
        {
        }
    }
}
