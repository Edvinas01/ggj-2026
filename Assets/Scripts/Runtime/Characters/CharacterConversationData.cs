using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    [Serializable]
    internal sealed class CharacterConversationData
    {
        [SerializeField]
        private List<CharacterMessageData> messages;

        public IEnumerable<CharacterMessageData> Messages => messages;

        public int MessageCount => messages.Count;

        public int ConversedCount { get; set; }

        public bool IsAnyBlurbs => messages.Any(m => m.MessageType == CharacterMessageType.RandomBlurb);

        public void RemoveMessage(CharacterMessageData message)
        {
            messages.Remove(message);
        }

        public void ClearMessages()
        {
            messages.Clear();
        }
    }
}
