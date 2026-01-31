namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal readonly struct ConversationChoice
    {
        public CharacterMessageType MessageType { get; }

        public bool IsCorrect { get; }

        public string Content { get; }

        public ConversationChoice(CharacterMessageType messageType, bool isCorrect, string content)
        {
            MessageType = messageType;
            IsCorrect = isCorrect;
            Content = content;
        }
    }
}
