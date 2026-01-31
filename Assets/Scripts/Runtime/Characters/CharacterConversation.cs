using System;
using System.Collections.Generic;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    [Serializable]
    internal sealed class CharacterConversation
    {
        [SerializeField]
        private List<CharacterMessage> messages;

        public IEnumerable<CharacterMessage> Messages => messages;
    }
}
