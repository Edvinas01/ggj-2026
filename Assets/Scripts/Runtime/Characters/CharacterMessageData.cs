using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    [Serializable]
    internal sealed class CharacterMessageData
    {
        [SerializeField]
        private CharacterMessageType messageType = CharacterMessageType.CorrectIncorrect;

        [SerializeField]
        private string content;

        [SerializeField]
        private List<string> correctChoices;

        [SerializeField]
        private List<string> incorrectChoices;

        public CharacterMessageType MessageType => messageType;

        public string Content => content;

        public IEnumerable<string> CorrectChoices => correctChoices;

        public IEnumerable<string> IncorrectChoices => incorrectChoices;
    }
}
