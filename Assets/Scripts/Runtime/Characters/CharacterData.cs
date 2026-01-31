using System.Collections.Generic;
using RIEVES.GGJ2026.Core.Constants;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + "Data_Character",
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Character Data",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    internal sealed class CharacterData : ScriptableObject
    {
        [Header("Identification")]
        [SerializeField]
        private string characterName;

        [Header("Visuals")]
        [SerializeField]
        private Texture2D frontTexture;

        [SerializeField]
        private Texture2D backTexture;

        [Header("Chatting")]
        [SerializeField]
        private CharacterConversation conversation;

        public string CharacterName => characterName;

        public Texture2D FrontTexture => frontTexture;

        public Texture2D BackTexture => backTexture;

        public CharacterConversation Conversation => conversation;
    }
}
