using System;
using System.Collections.Generic;
using RIEVES.GGJ2026.Core.Views;

namespace RIEVES.GGJ2026.Runtime.Characters
{
    internal sealed class ConversationViewController : ViewController<ConversationView>
    {
        public event Action<ConversationChoice> OnChoiceSelected;

        public void Initialize(string title, string content, IReadOnlyList<ConversationChoice> choices)
        {
            View.TitleText = title;
            View.ContentText = content;

            View.ClearChoices();

            foreach (var choice in choices)
            {
                View.AddChoice(choice, () => OnChoiceSelected?.Invoke(choice));
            }
        }
    }
}
