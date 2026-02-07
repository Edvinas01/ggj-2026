using RIEVES.GGJ2026.Runtime.Characters;

namespace RIEVES.GGJ2026.Runtime.Stats
{
    internal readonly struct ConversationStat
    {
        public CharacterData CharacterData { get; }

        public ConversationController.ConversationResult Result { get; }

        public ConversationStat(CharacterData characterData, ConversationController.ConversationResult result)
        {
            CharacterData = characterData;
            Result = result;
        }
    }
}
