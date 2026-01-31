namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal readonly struct ConversationChoice
    {
        public bool IsCorrect { get; }

        public string Content { get; }

        public ConversationChoice(bool isCorrect, string content)
        {
            IsCorrect = isCorrect;
            Content = content;
        }
    }
}
