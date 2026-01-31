using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationController : MonoBehaviour
    {
        [SerializeField]
        private ConversationViewController viewController;

        public void StartConversation(CharacterActor character)
        {
            viewController.Initialize(title: character.Name, content: "Work in progress...");
            viewController.ShowView();
        }

        public void StopConversation()
        {
            viewController.HideView();
        }
    }
}
