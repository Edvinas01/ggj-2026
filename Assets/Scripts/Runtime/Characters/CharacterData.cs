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

        [Header("Resources")]
        [Min(0)]
        [SerializeField]
        private int addsAlcohol = 10;

        [Min(0)]
        [SerializeField]
        private int removesAlcohol = 10;

        [Header("Visuals")]
        [SerializeField]
        private Texture2D frontTexture;

        [SerializeField]
        private Texture2D backTexture;

        [SerializeField]
        private List<Texture2D> handTextures;

        [Header("Chatting")]
        [SerializeField]
        private CharacterConversationData conversation;

        public string CharacterName => characterName;

        public int AddsAlcohol => addsAlcohol;

        public int RemovesAlcohol => removesAlcohol;

        public Texture2D FrontTexture => frontTexture;

        public Texture2D BackTexture => backTexture;

        public IEnumerable<Texture2D> HandTextures => handTextures;

        public CharacterConversationData ConversationData => conversation;
    }
}
