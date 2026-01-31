using System;
using System.Collections.Generic;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    [Serializable]
    internal sealed class CharacterConversationData
    {
        [SerializeField]
        private List<CharacterMessageData> messages;

        public IEnumerable<CharacterMessageData> Messages => messages;
    }
}
