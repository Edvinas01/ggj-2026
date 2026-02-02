using System.Collections.Generic;
using FMODUnity;
using RIEVES.GGJ2026.Core.Constants;
using UnityEngine;
using UnityEngine.Serialization;

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
        private List<Texture2D> giveHandTextures;

        [SerializeField]
        private List<Texture2D> punchHandTextures;

        [Header("Chatting")]
        [SerializeField]
        private CharacterConversationData conversation;

        [SerializeField]
        private float conversationDuration = 15f;

        [SerializeField]
        private float blurbDuration = 9f;

        [SerializeField]
        private float agitatedConversationDuration = 4f;

        [Header("Audio")]
        [SerializeField]
        private EventReference voiceFmodEvent;

        [SerializeField]
        private EventReference footstepFmodEvent;

        [SerializeField]
        private EventReference marozFmodEvent;

        [SerializeField]
        private EventReference marozVoiceFmodEvent;

        [Header("Behaviour Data")]
        [SerializeField]
        private List<ActivityPatiencePair> activityPatience;

        [SerializeField]
        private float calmMoveSpeed = 0.3f;

        [SerializeField]
        private float agitatedMoveSpeed = 1f;

        public string CharacterName => characterName;

        public int AddsAlcohol => addsAlcohol;

        public int RemovesAlcohol => removesAlcohol;

        public Texture2D FrontTexture => frontTexture;

        public Texture2D BackTexture => backTexture;

        public IEnumerable<Texture2D> GiveHandTextures => giveHandTextures;

        public IEnumerable<Texture2D> PunchHandTextures => punchHandTextures;

        public CharacterConversationData ConversationData => conversation;
        public float ConversationDuration => conversationDuration;
        public float BlurbDuration => blurbDuration;
        public float AgitatedConversationDuration => agitatedConversationDuration;
        public List<ActivityPatiencePair> ActivityPatience => activityPatience;
        public float CalmMoveSpeed => calmMoveSpeed;
        public float AgitatedMoveSpeed => agitatedMoveSpeed;

        public EventReference VoiceFmodEvent => voiceFmodEvent;
        public EventReference FootstepFmodEvent => footstepFmodEvent;
        public EventReference MarozFmodEvent => marozFmodEvent;
        public EventReference MarozVoiceFmodEvent => marozVoiceFmodEvent;
    }
}
